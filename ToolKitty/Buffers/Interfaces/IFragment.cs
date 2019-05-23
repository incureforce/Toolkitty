using System;

namespace System.Buffers
{
    public interface IFragment<T> : IDisposable
    {
        int Length { get; }

        ArraySegment<T> GetSegment();

        ArraySegment<T> GetSegment(int offset);

        ArraySegment<T> GetSegment(int offset, int count);
    }
}
