using System.IO;

namespace Nikos.Collections.HD_Engine
{
    ///<summary>
    ///</summary>
    ///<param name="stream"></param>
    ///<param name="offset"></param>
    ///<param name="args"></param>
    public delegate Allocator ResolveEngineMethod(Stream stream, long offset, params object [] args);
   
    ///<summary>
    ///</summary>
    ///<param name="sender"></param>
    ///<param name="currentPosition"></param>
    public delegate void AllocateEventHandler(object sender, long currentPosition);
    
    ///<summary>
    ///</summary>
    ///<param name="sender"></param>
    ///<param name="currentPosition"></param>
    public delegate void UnAllocateEventHandler(object sender, long currentPosition);
    
    ///<summary>
    ///</summary>
    public abstract class Allocator : IAllocator
    {
        private Stream _stream;
        protected long _offset;

        public event AllocateEventHandler OnAllocatePosition;
        public event UnAllocateEventHandler OnUnAllocatePosition;

        protected void InvokeOnUnAllocatePosition(long currentposition)
        {
            UnAllocateEventHandler handler = OnUnAllocatePosition;
            if (handler != null) handler(this, currentposition);
        }
        protected void InvokeOnAllocatePosition(long currentePosition)
        {
            AllocateEventHandler handler = OnAllocatePosition;
            if (handler != null) handler(this, currentePosition);
        }

        protected Allocator(Stream stream, long offset = 0)
        {
            _stream = stream;
            _offset = offset;
        }

        #region Implementation of IAllocator

        public abstract long Allocate(long blockSize);
        public abstract void UnAllocate(long offset, long blockSize);
        public abstract long Offset { get; protected set; }
        public Stream Stream { get { return _stream; } }

        #endregion
    }
}