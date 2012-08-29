using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Nikos.Cache;
using Nikos.Extensions.Collections;

namespace Nikos.Collections.HD_Engine
{

    ///<summary>
    ///Represent a B_Tree structure
    ///</summary>
    ///<typeparam name="T">Generic parameter</typeparam>
    public class B_Tree<T> : ICollection<T>, IDisposable where T : NodeItem, IComparable<T>, ISizable
    {
        #region protected
        
        /// <summary>
        /// Get access to memory cache
        /// </summary>
        protected MemoryCache<INode<T>> cache;


        /// <summary>
        /// Degree of tree
        /// </summary>
        protected int degree;
        
        /// <summary>
        /// File of tree asociated
        /// </summary>
        protected Stream file;

        protected Allocator _engine;

        protected ResolveEngineMethod engineMethod;

        /// <summary>
        /// The size of nodes
        /// </summary>
        protected int nodeSize;
        
        /// <summary>
        /// Indicate optimization with cache
        /// </summary>
        protected bool optimezedWithCache;
        
        /// <summary>
        /// The root of tree
        /// </summary>
        protected INode<T> root;
        
        /// <summary>
        /// Key size
        /// </summary>
        protected int tSize;
        
        protected B_Tree()
        {
        }

        //---------------------------------------------------------------

        /// <summary>
        /// Get a head size
        /// </summary>
        /// <returns></returns>
        protected virtual int GetHeadSize()
        {
            return BT_Head.Size;
        }

        /// <summary>
        /// Get the node size
        /// </summary>
        /// <returns></returns>
        protected virtual int GetNodeSize()
        {
            return (tSize * (2 * degree - 1)) + (ADRESS_SIZE * 2 * degree) + 8;
        }

        /// <summary>
        /// Get a instance of generic parameter
        /// </summary>
        /// <returns></returns>
        protected virtual T GetTInstance()
        {
            //we can make this, because T have a default costructor
            return (T)Activator.CreateInstance(typeof(T));
        }

        /// <summary>
        /// Inicialize the memory cache
        /// </summary>
        /// <param name="capacity">Capacity of memory in kilo bytes</param>
        protected virtual void InitializeCache(int capacity)
        {
            if (optimezedWithCache)
                cache = MemoryCache<INode<T>>.Get_Intance(capacity);
        }

        /// <summary>
        /// Write the head of tree into asociate stream
        /// </summary>
        protected virtual void WriteHead()
        {
            Head.RootPosition = root.Location;
            Head.EnginePosition = _engine.Offset; //HddEngine.Get_Intance(file.Name).CurrentePosition;
            file.Position = Head.PositionOnStream;
            file.Write(Head.ToByteArray(), 0, GetHeadSize());
        }

        protected virtual INode<T> CreateNode(long location = 0)
        {
            return new B_TreeNode<T>(degree, location, nodeSize);
        }

        /// <summary>
        /// Write a node into asociate stream
        /// </summary>
        /// <param name="x"></param>
        protected virtual void DiskWrite(INode<T> x)
        {
            if (optimezedWithCache)
            {
                cache.Add(this, x, _DiskWrite);
            }
            else _DiskWrite(x);
        }

        /// <summary>
        /// Read one node from streams
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        protected virtual INode<T> DiskRead(Stream stream, long position)
        {
            if (optimezedWithCache)
            {
                INode<T> x = cache.Find(this, t => t.Location, position);

                return x ?? _DiskRead(stream, position);
            }
            return _DiskRead(stream, position);
        }

        /// <summary>
        /// Performance a binary searsh into keys of values
        /// </summary>
        /// <param name="values">Value over make the search</param>
        /// <param name="value">Value to search</param>
        /// <param name="start">zero-based index to star the search</param>
        /// <param name="end">zero-based index to end the search</param>
        /// <returns></returns>
        protected virtual int BinarySearch(T[] values, T value, int start, int end)
        {
            if (start > end) return end;
            int medio = (start + end) >> 1;
            if (value.CompareTo(values[medio]) == 0) return medio;//Base
            return value.CompareTo(values[medio]) > 0 ? BinarySearch(values, value, medio + 1, end) : BinarySearch(values, value, start, medio - 1);
        }

        /// <summary>
        /// In orden walk over one node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected IEnumerable<T> InOrderTreeWalk(INode<T> node)
        {
            if (!node.IsLeaf)
                foreach (T item in InOrderTreeWalk(DiskRead(file, node.Childrens[0])))
                    yield return item; // Recorre en simétrico el primer hijo
            for (int i = 1; i <= node.KeysCount; i++)
            {
                yield return (node.Keys[i - 1]); // Imprime la llave (i-1)-esima
                if (!node.IsLeaf)
                    foreach (T item in InOrderTreeWalk(DiskRead(file, node.Childrens[i])))
                        yield return item; // Recorre en entreorde el hijo i-esimo
            }
        }

        /// <summary>
        /// In back orden walk over one node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected IEnumerable<T> InBackOrderTreeWalk(INode<T> node)
        {
            if (!node.IsLeaf)
                foreach (T item in InBackOrderTreeWalk(DiskRead(file, node.Childrens[node.KeysCount])))
                    yield return item; // Recorre en simétrico el primer hijo
            for (int i = node.KeysCount; i > 0; i--)
            {
                yield return (node.Keys[i - 1]); // Imprime la llave (i-1)-esima
                if (!node.IsLeaf)
                    foreach (T item in InBackOrderTreeWalk(DiskRead(file, node.Childrens[i - 1])))
                        yield return item; // Recorre en entreorde el hijo i-esimo
            }
        }

        protected Allocator ResolveEngine(Stream stream, long enginePosition)
        {
            return this.engineMethod(stream, enginePosition);
        }

        #endregion

        #region Private

        private const int ADRESS_SIZE = 8;

        

        private void Insert_NonFull(INode<T> node, T value) //hacerlo iterativo.
        {
            INode<T> nodeResult = node;
            while (true)
                if (nodeResult.IsLeaf)
                {
                    if (nodeResult.KeysCount == 0)
                    {
                        nodeResult.Keys[0] = value;
                        nodeResult.KeysCount++;
                        DiskWrite(nodeResult);
                        return;
                    }
                    bool flag = true;
                    int index = BinarySearch(nodeResult.Keys, value, 0, nodeResult.KeysCount - 1);
                    if (index >= 0 && nodeResult.Keys[index].CompareTo(value) == 0)
                        flag = false;

                    if (flag)
                    {
                        int i;
                        for (i = nodeResult.KeysCount; i > 0; i--) 
                        {
                            if (value.CompareTo(nodeResult.Keys[i - 1]) < 0)
                                nodeResult.Keys[i] = nodeResult.Keys[i - 1];
                            else break;
                        }
                        nodeResult.Keys[i] = value;
                        nodeResult.KeysCount++;
                        DiskWrite(nodeResult);
                    }
                    return;
                }
                else
                {
                    bool flag = true;
                    int index = BinarySearch(nodeResult.Keys, value, 0, nodeResult.KeysCount - 1);
                    if (index >= 0 && nodeResult.Keys[index].CompareTo(value) == 0)
                    {
                        flag = false;
                    }
                    if (flag)
                    {
                        INode<T> child = DiskRead(file, nodeResult.Childrens[index + 1]);
                        INode<T> aux = null;
                        if (child.KeysCount == 2 * degree - 1) aux = Split_Child(nodeResult, index + 1, child);
                        if (aux != null) //si child estaba lleno tengo que decidir entonces por cual de los dos hijos(child o aux) debo vajar.
                        {
                            if (value.CompareTo(nodeResult.Keys[index + 1]) == 0) return;
                            if (value.CompareTo(nodeResult.Keys[index + 1]) > 0) child = aux;
                        } //si no es por child.
                        nodeResult = child;
                    }
                    else return;
                }
        }

        private INode<T> Split_Child(INode<T> father, int pos, INode<T> child) //root.Degree = t 
        {
           // HddEngine hdd = HddEngine.Get_Intance(file.Name);

            INode<T> aux = new B_TreeNode<T>(root.Degree, nodeSize) {/*IsLeaf = child.IsLeaf,*/Location = _engine.Allocate(nodeSize), KeysCount = degree - 1, ChildrensCount = (!child.IsLeaf) ? degree : 0 };

            for (int i = 0; i < root.Degree - 1; i++) //capia las llaves que estaban en child y que deben estar en aux.
                aux.Keys[i] = child.Keys[root.Degree + i];

            if (!child.IsLeaf)
                for (int i = 0; i < root.Degree; i++) //capia los hijos que estaban en child y que deben estar en aux.
                    aux.Childrens[i] = child.Childrens[root.Degree + i];

            child.KeysCount = degree - 1;
            child.ChildrensCount = (!child.IsLeaf) ? degree : 0;

            for (int i = father.ChildrensCount; i > pos + 1; i--)
                father.Childrens[i] = father.Childrens[i - 1];

            father.Childrens[pos + 1] = aux.Location;

            for (int i = father.KeysCount; i > pos; i--)
                father.Keys[i] = father.Keys[i - 1];

            father.Keys[pos] = child.Keys[degree - 1];
            father.KeysCount = father.KeysCount + 1;
            father.ChildrensCount = father.ChildrensCount + 1;

            DiskWrite(father);
            DiskWrite(child);
            DiskWrite(aux);

            return aux;
        }

        private void _DiskWrite(params object[] args)
        {
            var x = (INode<T>)args[0];

            var wBuffer = new List<byte>();
            wBuffer.AddRange(BitConverter.GetBytes(x.KeysCount));
            wBuffer.AddRange(BitConverter.GetBytes(x.ChildrensCount));

            for (int i = 0; i < x.ChildrensCount; i++)
                wBuffer.AddRange(BitConverter.GetBytes(x.Childrens[i]));

            for (int i = 0; i < x.KeysCount; i++)
            {
                byte[] temp = x.Keys[i].ToByteArray();

                //verificando que correspondan los tamannos
                if (temp.Length != tSize)
                    throw new InvalidOperationException(x.Keys[i].ToString());

                wBuffer.AddRange(temp);
            }

            //verificando que el buffer tenga el tamanno correcto
            if (wBuffer.Count > nodeSize)
                throw new InvalidOperationException("buffer");

            //rellenando el buffer
            wBuffer.AddRange(new byte[nodeSize - wBuffer.Count]);

            file.Position = x.Location;
            file.Write(wBuffer.ToArray(), 0, wBuffer.Count);
        }

        private INode<T> _DiskRead(Stream stream, long position)
        {
            var result = new B_TreeNode<T>(degree, position, nodeSize);
            stream.Position = position;
            var rBuffer = new byte[nodeSize];
            stream.Read(rBuffer, 0, nodeSize);

            int size = 4;
            int index = 0;
            result.KeysCount = BitConverter.ToInt32(rBuffer.SubSec(0, size - 1).ToArray(), 0);
            index += size;

            result.ChildrensCount = BitConverter.ToInt32(rBuffer.SubSec(index, index + size - 1).ToArray(), 0);
            index += size;

            size = 8;
            for (int i = 0; i < result.ChildrensCount; i++)
            {
                result.Childrens[i] = BitConverter.ToInt64(rBuffer.SubSec(index, index + size - 1).ToArray(), 0);
                index += size;
            }

            size = tSize;
            for (int i = 0; i < result.KeysCount; i++)
            {
                T item = GetTInstance();
                item.LoadFromByteArray(rBuffer.SubSec(index, index + size - 1).ToArray());
                result.Keys[i] = item;
                index += size;
            }

            return result;
        }

        // --------------------------------------------------------------------

        private int Contains(T value, INode<T> x, out INode<T> result)
        {
            int index = BinarySearch(x.Keys, value, 0, x.KeysCount - 1);
            if (index >= 0 && x.Keys[index].CompareTo(value) == 0)
            {
                result = x;
                return index;
            }
            if (!x.IsLeaf)
            {
                INode<T> temp = DiskRead(file, x.Childrens[index + 1]);
                int t = Contains(value, temp, out result);
                if (t >= 0)
                    return t;
            }
            result = null;
            return -1;
        }

        private IEnumerable<T> Between(INode<T> x, T value1, T value2)
        {
            if (value1 == null) throw new ArgumentNullException("value1");
            if (value2 == null) throw new ArgumentNullException("value2");
            if (value1.CompareTo(value2) <= 0)
            {
                int index1 = BinarySearch(x.Keys, value1, 0, x.KeysCount - 1);
                int index2 = BinarySearch(x.Keys, value2, 0, x.KeysCount - 1);

                if (index1 != index2)
                {
                    if (index1 >= 0 && x.Keys[index1].CompareTo(value1) == 0)
                        yield return x.Keys[index1];
                    index1++;
                    if (!x.IsLeaf)
                        foreach (var item in MayorQue(value1, DiskRead(file, x.Childrens[index1])))
                            yield return item;
                    for (int i = index1; i < x.KeysCount; i++)
                        if (x.Keys[i].CompareTo(value2) <= 0)
                        {
                            yield return x.Keys[i];
                            if (!x.IsLeaf)
                                foreach (var item in MenorQue(value2, DiskRead(file, x.Childrens[i + 1])))
                                    yield return item;
                        }
                        else break;
                }
                else//si tienen que bajar por el mismo nodo hacemos lo mismo sobre el nodo comun
                {
                    if (index1 >= 0 && x.Keys[index1].CompareTo(value1) == 0)
                        yield return x.Keys[index1];
                    if (!x.IsLeaf)
                        foreach (var item in Between(DiskRead(file, x.Childrens[index1 + 1]), value1, value2))
                            yield return item;
                }
            }
        }

        private IEnumerable<T> MayorQue(T value, INode<T> x)
        {
            foreach (T t in InBackOrderTreeWalk(x))
            {
                if (t.CompareTo(value) >= 0)
                    yield return t;
                else
                    yield break;
            }
        }

        private IEnumerable<T> MenorQue(T value, INode<T> x)
        {
            foreach (T t in InOrderTreeWalk(x))
            {
                if (t.CompareTo(value) <= 0)
                    yield return t;
                else
                    yield break;
            }
        }

        #region BTree_Delete

        private void TranslateValueLeft(INode<T> father, int valuePos, INode<T> childTo, INode<T> childFrom)
        {
            if (childFrom.KeysCount == degree - 1 || childTo.KeysCount > degree - 1) throw new ArgumentException();

            childTo.Keys[childTo.KeysCount] = father.Keys[valuePos];
            childTo.KeysCount++;
            father.Keys[valuePos] = childFrom.Keys[0];
            childFrom.KeysCount--;

            if (!childTo.IsLeaf)
            {
                childTo.Childrens[childTo.ChildrensCount] = childFrom.Childrens[0];
                childTo.ChildrensCount++;
                childFrom.ChildrensCount--;
            }

            Array.Copy(childFrom.Keys, 1, childFrom.Keys, 0, childFrom.KeysCount);

            if (!childFrom.IsLeaf)

                Array.Copy(childFrom.Childrens, 1, childFrom.Childrens, 0, childFrom.ChildrensCount);

            DiskWrite(father);
            DiskWrite(childFrom);
            DiskWrite(childTo);
        } //Estoy escribiendo al archivo aquí.

        private void TranslateValueRight(INode<T> father, int valuePos, INode<T> childFrom, INode<T> childTo)
        {
            if (childFrom.KeysCount == degree - 1 || childTo.KeysCount > degree - 1) throw new ArgumentException();

            Array.Copy(childTo.Keys, 0, childTo.Keys, 1, childTo.KeysCount);
            childTo.Keys[0] = father.Keys[valuePos];
            father.Keys[valuePos] = childFrom.Keys[childFrom.KeysCount - 1];
            childTo.KeysCount++;
            childFrom.KeysCount--;

            if (!childTo.IsLeaf)
            {
                Array.Copy(childTo.Childrens, 0, childTo.Childrens, 1, childTo.ChildrensCount);
                childTo.Childrens[0] = childFrom.Childrens[childFrom.ChildrensCount - 1];
                childTo.ChildrensCount++;
                childFrom.ChildrensCount--;
            }

            DiskWrite(father);
            DiskWrite(childFrom);
            DiskWrite(childTo);
        } //Estoy escribiendo al archivo aquí.

        private void Merge(INode<T> father, int valuePos, INode<T> leftChild, INode<T> rightChild)
        {
            if ( /*father.KeysCount < Degree ||*/ leftChild.KeysCount >= degree || rightChild.KeysCount >= degree) throw new ArgumentException();

            leftChild.Keys[leftChild.KeysCount] = father.Keys[valuePos];
            Array.Copy(rightChild.Keys, 0, leftChild.Keys, leftChild.KeysCount + 1, rightChild.KeysCount);
            leftChild.KeysCount = 2 * degree - 1;

            if (!leftChild.IsLeaf)
            {
                Array.Copy(rightChild.Childrens, 0, leftChild.Childrens, leftChild.ChildrensCount, rightChild.ChildrensCount);
                leftChild.ChildrensCount = 2 * degree;
            }

            Array.Copy(father.Keys, valuePos + 1, father.Keys, valuePos, (father.KeysCount - (valuePos + 1)));
            father.KeysCount--;

            Array.Copy(father.Childrens, valuePos + 2, father.Childrens, valuePos + 1, father.ChildrensCount - (valuePos + 2));
            father.ChildrensCount--;

            DiskWrite(father);
            DiskWrite(leftChild);
            //HddEngine hdd = HddEngine.Get_Intance(file.Name);
            _engine.UnAllocate(rightChild.Location, nodeSize);
        } //une dos nodos en uno, siendo el resultante una modificación de leftchild.      

        private T PredecessorOf(INode<T> node)
        {
            INode<T> result = node;
            while (true)

                if (result.IsLeaf) return result.Keys[result.KeysCount - 1];
                else result = DiskRead(file, result.Childrens[result.ChildrensCount - 1]);
        }

        private T SuccessorOf(INode<T> node)
        {
            INode<T> result = node;
            while (true)

                if (result.IsLeaf) return result.Keys[0];
                else result = DiskRead(file, result.Childrens[0]);
        }

        private void Delete(INode<T> node, T value)
        {
            int index = BinarySearch(node.Keys, value, 0, node.KeysCount - 1);
            if (index < 0 || node.Keys[index].CompareTo(value) < 0) index++;
            if (node.IsLeaf) //eliminando de una hoja.
            {
                if (node.KeysCount > 0 && index < node.KeysCount && value.CompareTo(node.Keys[index]) == 0)
                {
                    Array.Copy(node.Keys, index + 1, node.Keys, index, node.KeysCount - (index + 1));
                    node.KeysCount--;
                    DiskWrite(node);
                }
                return;
            }
            if (index < node.KeysCount && value.CompareTo(node.Keys[index]) == 0) //Eliminando si estoy en el nodo donde esté el valor.
            {
                INode<T> aux = DiskRead(file, node.Childrens[index]); //y en el Introduction.
                if (aux.KeysCount >= degree)
                {
                    T predecessor = PredecessorOf(aux);
                    Delete(aux, predecessor);
                    node.Keys[index] = predecessor;
                    DiskWrite(node);
                    return;
                }
                INode<T> aux2 = DiskRead(file, node.Childrens[index + 1]); //ver aquí si no estoy fuera de rango. Al parecer no.//z en el Introduction.
                if (aux2.KeysCount >= degree)
                {
                    T successor = SuccessorOf(aux2);
                    Delete(aux2, successor);
                    node.Keys[index] = successor;
                    DiskWrite(node);
                    return;
                }
                Merge(node, index, aux, aux2);
                Delete(aux, value);
                return;
            }
            else //Si no estoy en el nodo del valor, preparar al hijo por el que debo bajar.
            {
                INode<T> aux = DiskRead(file, node.Childrens[index]);
                if (aux.KeysCount >= degree)
                {
                    Delete(aux, value);
                    return;
                }

                INode<T> aux2 = null, aux3 = null;
                if (index >= 1) aux2 = DiskRead(file, node.Childrens[index - 1]);
                if (index + 1 < node.ChildrensCount) aux3 = DiskRead(file, node.Childrens[index + 1]);

                if (aux2 != null && aux2.KeysCount >= degree)
                {
                    TranslateValueRight(node, index - 1, aux2, aux);
                    Delete(aux, value);
                    return;
                }
                if (aux3 != null && aux3.KeysCount >= degree)
                {
                    TranslateValueLeft(node, index, aux, aux3);
                    Delete(aux, value);
                    return;
                }
                if (aux3 != null)
                {
                    Merge(node, index, aux, aux3);
                    Delete(aux, value);
                    return;
                }
                if (aux2 != null)
                {
                    Merge(node, index - 1, aux2, aux);
                    Delete(aux2, value);
                    return;
                }
            }
        }

        #endregion Private B_Tree Delete

        // ----------------------------------------------------------------------

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new B_Tree
        /// </summary>
        /// <param name="degree">Degree of tree</param>
        /// <param name="stream">Stream asociated to B_Tree</param>
        /// <param name="engine"></param>
        /// <param name="usingCache">Indicated optimization with cache</param>
        /// <param name="cacheCapacity">Capacy of cache, if cache was created this parameter is ignorer</param>
        public B_Tree(int degree, Stream stream, Allocator engine, bool usingCache, int cacheCapacity = 2048)
        {
            if (degree <= 1)
                throw new ArgumentException("Invalid degree");
            if (stream == null)
                throw new ArgumentNullException("stream");

            optimezedWithCache = usingCache;
            InitializeCache(cacheCapacity); //opteniendo acceso a la memoria cache

            T value = GetTInstance();
            tSize = (int)value.get_Size();
            this.degree = degree;
            nodeSize = GetNodeSize();
            file = stream;
            _engine = engine;
            INode<T> aux = CreateNode();
            Head = new BT_Head { Degree = degree, NodeSize = nodeSize, PositionOnStream = _engine.Allocate(GetHeadSize()), tSize = tSize, OptimedWithCache = optimezedWithCache };
            aux.Location = _engine.Allocate(nodeSize);
            root = aux;
            DiskWrite(root);
        }

        /// <summary>
        /// Create a new B_Tree
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="engine"></param>
        /// <param name="usingCache"></param>
        /// <param name="cacheCapacity"></param>
        public B_Tree(Stream stream, Allocator engine, bool usingCache, int cacheCapacity = 2048)
            : this(500, stream, engine, usingCache, cacheCapacity)
        {
        }

        /// <summary>
        /// Create a B_Tree 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="head"></param>
        /// <param name="engineMethod"></param>
        protected B_Tree(FileStream stream, BT_Head head, ResolveEngineMethod engineMethod)
        {
            tSize = head.tSize;
            nodeSize = head.NodeSize;
            degree = head.Degree;
            file = stream;
            this.engineMethod = engineMethod;
            //HddEngine.Initialize(file.Name, head.EnginePosition);
            _engine = ResolveEngine(file, head.EnginePosition);
            optimezedWithCache = head.OptimedWithCache;
            InitializeCache(2048);

            Head = head;
            root = DiskRead(file, head.RootPosition);
        }

        #endregion

        //------------------------------------------------------------------------

        ///<summary>
        /// Carga el B_Tree dado un archivo y la posicion de la cabecera
        ///</summary>
        ///<param name="stream"></param>
        ///<param name="headPosition"></param>
        ///<param name="engineMethod"></param>
        ///<returns></returns>
        public static B_Tree<T> Load(FileStream stream, long headPosition, ResolveEngineMethod engineMethod)
        {
            try
            {
                stream.Position = headPosition;
                var head = new BT_Head();
                var buffer = new byte[BT_Head.Size];
                stream.Read(buffer, 0, BT_Head.Size);
                head.LoadFromByteArray(buffer);

                return new B_Tree<T>(stream, head, engineMethod);
            }
            catch
            {
                stream.Close();
                throw new InvalidOperationException("The format is not correct");
            }
        }

        //------------------------------------------------------------------------

        #region Public

        /// <summary>
        /// In orden walk over BTree
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> InOrderTreeWalk()
        {
            return InOrderTreeWalk(root);
        }

        /// <summary>
        /// In back orden walk over BTree
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> InBackOrderTreeWalk()
        {
            return InBackOrderTreeWalk(root);
        }

        ///<summary>
        ///</summary>
        ///<param name="value"></param>
        ///<returns></returns>
        public IEnumerable<T> LessThan(T value)
        {
            foreach (T item in InOrderTreeWalk())
            {
                if (item.CompareTo(value) < 0)
                    yield return item;
                else yield break;
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="value"></param>
        ///<returns></returns>
        public IEnumerable<T> GreatherThan(T value)
        {
            foreach (T item in InBackOrderTreeWalk())
            {
                if (item.CompareTo(value) > 0)
                    yield return item;
                else yield break;
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="value1"></param>
        ///<param name="value2"></param>
        ///<returns></returns>
        public IEnumerable<T> Between(T value1, T value2)
        {
            if (value1 == null) throw new ArgumentNullException("value1");
            if (value2 == null) throw new ArgumentNullException("value2");
            return Between(root, value1, value2);
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Head of tree
        /// </summary>
        public BT_Head Head { get; protected set; }

        /// <summary>
        /// Determine if tree is optimezed with cache
        /// </summary>
        public bool OptimezedWithCache
        {
            get { return optimezedWithCache; }
        }

        /// <summary>
        /// Get the maximun item on tree
        /// </summary>
        public T Max
        {
            get
            {
                INode<T> x = root;

                while (true)
                {
                    if (x.IsLeaf)
                        return x.Keys[x.KeysCount - 1];
                    x = DiskRead(file, x.Childrens[x.ChildrensCount - 1]);
                }
            }
        }

        /// <summary>
        /// Get the minimum item on tree
        /// </summary>
        public T Min
        {
            get
            {
                INode<T> x = root;

                while (true)
                {
                    if (x.IsLeaf)
                        return x.Keys[0];
                    x = DiskRead(file, x.Childrens[0]);
                }
            }
        }

        #endregion

        #region Implementation of IEnumerable

        /// <summary>
        ///                     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///                     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return InOrderTreeWalk().GetEnumerator();
        }

        /// <summary>
        ///                     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///                     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        ///                     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            //escribiendo la cabecera
            WriteHead();

            if (optimezedWithCache)
                //eliminando todos mis elementos del cache y a la misma vez escribiendolos hacia el disco
                cache.Clear(this);
        }

        //-------------------------------------------------------------------------------------

        #endregion

        #region Implementation of ICollection<T>

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///                 </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        ///                 </exception>
        public void Add(T item)
        {
            if (root.KeysCount == 2 * root.Degree - 1)
            {
                //HddEngine hdd = HddEngine.Get_Intance(file.Name);

                INode<T> oldRoot = root;
                INode<T> newRoot = new B_TreeNode<T>(degree, nodeSize) { KeysCount = 0, ChildrensCount = 1, Location = _engine.Allocate(nodeSize) };
                newRoot.Childrens[0] = oldRoot.Location;
                root = newRoot;
                Split_Child(root, 0, oldRoot);
                Insert_NonFull(root, item);
            }
            else
                Insert_NonFull(root, item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. 
        ///                 </exception>
        public void Clear()
        {
           // var engine = HddEngine.Initialize(file.Name, 0);
            _engine = ResolveEngine(file, 0);
            Head.PositionOnStream = _engine.Allocate(BT_Head.Size);
            root = new B_TreeNode<T>(degree, nodeSize) { Location = _engine.Allocate(nodeSize) };
            cache.Clear(this, false);
            DiskWrite(root);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///                 </param>
        public bool Contains(T item)
        {
            INode<T> aux;
            return Contains(item, out aux) >= 0;
        }

        ///<summary>
        ///</summary>
        ///<param name="value"></param>
        ///<returns></returns>
        public T ContainsI(T value)
        {
            INode<T> result;
            int index = Contains(value, out result);

            return index >= 0 ? result.Keys[index] : default(T);
        }

        ///<summary>
        /// Verifi if a value is in tree
        ///</summary>
        ///<param name="value">value to search</param>
        ///<param name="x"> the out node where the value is</param>
        ///<returns>return a zero-based value than is the index of serached value into out node</returns>
        public int Contains(T value, out INode<T> x)
        {
            return Contains(value, root, out x);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.
        ///                 </param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.
        ///                 </param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.
        ///                 </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.
        ///                 </exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.
        ///                     -or-
        ///                 <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        ///                     -or-
        ///                     The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        ///                     -or-
        ///                     Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        ///                 </exception>ku
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (array.Rank > 1)
                throw new ArgumentException();

            var source = GetEnumerator();
            for (int i = arrayIndex; i < array.Length; i++)
            {
                source.MoveNext();
                array[i] = source.Current;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///                 </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        ///                 </exception>
        public bool Remove(T item)
        {
            INode<T> node;
            int result = Contains(item, root, out node);
            if (result >= 0)
            {
                Delete(root, item);
                if (root.ChildrensCount == 1)
                {
                    _engine.UnAllocate(root.Location, nodeSize);
                    root = DiskRead(file, root.Childrens[0]);
                }
            }
            return result >= 0;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// This property should not be called because is not optimized on this data-structure
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get { return (new List<T>(GetEnumerator().ToEnumerable())).Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion
    }
}