using System.Collections.Generic;
using System.IO;

namespace Nikos.Collections.HD_Engine
{
    public class DriveEngine : Allocator
    {
        private readonly Dictionary<float, List<long>> m_FatTable;

        private static float FromByteToKiloByte(long numberOfBytes)
        {
            return numberOfBytes / 1024f;
        }

        public DriveEngine(Stream stream, long offset = 0) : base(stream, offset)
        {
            m_FatTable = new Dictionary<float, List<long>>();
        }

        #region Overrides of Allocator

        public override long Allocate(long blockSize)
        {
            //si hay un bloque libre del tamanno deseado
            if (m_FatTable.ContainsKey(FromByteToKiloByte(blockSize)))
            {
                var x = m_FatTable[FromByteToKiloByte(blockSize)];
                long t = x[0];
                x.RemoveAt(0);
                if (x.Count == 0) m_FatTable.Remove(FromByteToKiloByte(blockSize));
                InvokeOnAllocatePosition(t);
                return t;
            }

            //si no hay ningun espacion disponible entonces se le da uno al final
            long result = Offset;
            Offset += blockSize + 1;
            InvokeOnAllocatePosition(Offset);
            return result;
        }

        public override void UnAllocate(long offset, long blockSize)
        {
            if (offset + blockSize + 1 == Offset)
            {
                Offset = offset;
                InvokeOnAllocatePosition(offset);
                return;
            }
            if (m_FatTable.ContainsKey(FromByteToKiloByte(blockSize)))
            {
                var p = m_FatTable[FromByteToKiloByte(blockSize)];

                //insertarlo ordenado
                for (int i = 0; i < p.Count; i++)
                    if (p[i] > offset)
                    {
                        p.Insert(i, offset);
                        InvokeOnUnAllocatePosition(offset);
                    }
            }
            else
            {
                var x = new List<long> { offset };
                m_FatTable.Add(FromByteToKiloByte(blockSize), x);
                InvokeOnUnAllocatePosition(offset);
            }
        }

        public override long Offset
        {
            get { return _offset; }
            protected set { _offset = value; }
        }

        #endregion
    }
}