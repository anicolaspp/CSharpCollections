using System.IO;

namespace Nikos.Collections.HD_Engine
{
    public interface IAllocator
    {
        long Allocate(long blockSize);
        void UnAllocate(long offset, long blockSize);
        long Offset { get; }
        Stream Stream { get; }
    }
}