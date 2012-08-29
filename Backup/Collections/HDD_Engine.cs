using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nikos.Collections.Advance.HD_Engine
{
    /// <summary>
    /// Encargada del acceso al disco duro.
    /// Encargada de la desfracmentacion del archivo.
    /// Clase a modo Singlenton.
    /// </summary>
    public class HDD_Engine
    {
        private static readonly Collections.Advance.Dictionary<string, HDD_Engine> m_Intances = new Collections.Advance.Dictionary<string, HDD_Engine>();
        private readonly Collections.Advance.Dictionary<long, List<long>> m_FatTable;

        /// <summary>     
        /// Create a new instance of HDD_Engine
        /// </summary>
        private HDD_Engine(string stream)
        {
            m_FatTable = new Collections.Advance.Dictionary<long, List<long>>();
            StreamPath = stream;
        }

        /// <summary>
        /// Return the currente position of blocks on the stream
        /// </summary>
        public long CurrentePosition { get; private set; }

        ///<summary>
        /// Ruta del stream asociado a la intancia del engine
        ///</summary>
        public string StreamPath { get; private set; }

        /// <summary>
        /// Obtiene la posicion donde se va a guardar el bloque y avanza a la proxima posicion.
        /// </summary>
        /// <param name="blockSize">Tamanno del bloque a guardar</param>
        /// <returns>Posicion donde se va a guardar el bloque</returns>
        public long Allocate(long blockSize)
        {
            //si hay un bloque libre del tamanno deseado
            if (m_FatTable.ContainsKey(blockSize))
            {
                var x = m_FatTable[blockSize];
                long t = x[0];
                x.RemoveAt(0);
                if (x.Count == 0) m_FatTable.Remove(blockSize);
                return t;
            }

            //si no hay ningun espacion disponible entonces se le da uno al final
            long result = CurrentePosition;
            CurrentePosition += blockSize + 1;
            return result;
        }

        /// <summary>
        /// Libera un bloque en el stream para que pueda ser utilizado en otro momento
        /// </summary>
        /// <param name="blockPosition">Posicion en donde comienza el bloque</param>
        /// <param name="blockSize">Tamanno del bloque</param>
        public void UnAllocate(long blockPosition, long blockSize)
        {
            if (m_FatTable.ContainsKey(blockSize))
            {
                var p = m_FatTable[blockSize];

                //insertarlo ordenado
                for (int i = 0; i < p.Count; i++)
                    if (p[i] > blockPosition)
                    {
                        p.Insert(i, blockPosition);
                        return;
                    }
            }

            var x = new List<long> { blockPosition };
            m_FatTable.Add(blockSize, x);
        }

        ///<summary>
        /// Get a instance of this class
        ///</summary>
        ///<returns></returns>
        public static HDD_Engine Get_Intance(string streamPath)
        {
            if (m_Intances.ContainsKey(streamPath))
                return m_Intances[streamPath];

            m_Intances.Add(streamPath, new HDD_Engine(streamPath));
            return m_Intances[streamPath];
        }

        ///<summary>
        /// Inicialize the intances of this class on determinate position
        ///</summary>
        ///<param name="stream"></param>
        ///<param name="position"></param>
        ///<returns></returns>
        public static HDD_Engine Initialize(string stream, long position)
        {
            if (m_Intances.ContainsKey(stream))
                m_Intances[stream] = new HDD_Engine(stream) { CurrentePosition = position };
            else
                m_Intances.Add(stream, new HDD_Engine(stream) { CurrentePosition = position });

            return m_Intances[stream];
        }
    }
}
