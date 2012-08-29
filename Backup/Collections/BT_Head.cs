using System;
using System.Collections.Generic;
using System.Linq;
using Nikos.Collections.Advance.HD_Engine;
using Nikos.Extensions.Collections;

namespace Nikos.Collections.Advance.HD_Engine
{
    public class BT_Head : NodeItem
    {
        /// <summary>
        /// Size of head of tree
        /// </summary>
        public static readonly int Size = 37;

        public int Degree { get; set; }
        public int NodeSize { get; set; }
        public int tSize { get; set; }
        public long RootPosition { get; set; }
        public long PositionOnStream { get; set; }
        public bool OptimedWithCache { get; set; }
        public long EnginePosition { get; set; }

        #region Overrides of NodeItem

        /// <summary>
        /// The position of last lecture on LoadFromByteArray method.
        /// Use in derived classes for continous read in override method
        /// </summary>
        protected int index;

        /// <summary>
        /// Get the bytes than represent a class.
        /// </summary>
        /// <returns></returns>
        public override byte[] ToByteArray()
        {
            var buffer = new List<byte>();
            buffer.AddRange(BitConverter.GetBytes(Degree));
            buffer.AddRange(BitConverter.GetBytes(NodeSize));
            buffer.AddRange(BitConverter.GetBytes(tSize));
            buffer.AddRange(BitConverter.GetBytes(RootPosition));
            buffer.AddRange(BitConverter.GetBytes(PositionOnStream));
            buffer.AddRange(BitConverter.GetBytes(OptimedWithCache));
            buffer.AddRange(BitConverter.GetBytes(EnginePosition));

            return buffer.ToArray();
        }

        /// <summary>
        /// Load to data-object the data passed as parameters
        /// </summary>
        /// <param name="data"></param>
        public override void LoadFromByteArray(byte[] data)
        {
            int size = 4;
            index = 0;

            Degree = BitConverter.ToInt32(data.SubSec(index, index + size - 1).ToArray(), 0);
            index += size;
            NodeSize = BitConverter.ToInt32(data.SubSec(index, index + size - 1).ToArray(), 0);
            index += size;
            tSize = BitConverter.ToInt32(data.SubSec(index, index + size - 1).ToArray(), 0);
            index += size;
            size = 8;
            RootPosition = BitConverter.ToInt64(data.SubSec(index, index + size - 1).ToArray(), 0);
            index += size;
            PositionOnStream = BitConverter.ToInt64(data.SubSec(index, index + size - 1).ToArray(), 0);
            index += size;
            size = 1;
            OptimedWithCache = BitConverter.ToBoolean(data.SubSec(index, index + size - 1).ToArray(), 0);
            index += size;
            size = 8;
            EnginePosition = BitConverter.ToInt64(data.SubSec(index, index + size - 1).ToArray(), 0);
            index += size;
        }

        #endregion
    }
}
