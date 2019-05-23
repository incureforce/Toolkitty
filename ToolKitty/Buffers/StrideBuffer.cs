using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Buffers
{
    public class StrideBuffer<T>
    {
        private class Fragment : IEquatable<Fragment>, IFragment<T>
        {
            public readonly T[] Buffer;

            public readonly int Offset;

            public readonly int Stride;

            public Fragment(T[] buffer, int offset, int stride) {
                Buffer = buffer;
                Offset = offset;
                Stride = stride;
            }

            public int Length { get; set; }

            public bool Usable => Length == 0;

            public ArraySegment<T> GetSegment() {
                var sliceOffset = Offset;
                var sliceLength = Length;

                return GetSlice(sliceOffset, sliceLength);
            }

            public ArraySegment<T> GetSegment(int offset) {
                var sliceOffset = Offset + offset;
                var sliceLength = Length - offset;

                return GetSlice(sliceOffset, sliceLength);
            }

            public ArraySegment<T> GetSegment(int offset, int length) {
                var sliceOffset = Offset + offset;
                var sliceLength = length;

                return GetSlice(sliceOffset, sliceLength);
            }

            private ArraySegment<T> GetSlice(int sliceOffset, int sliceLength) {
                if (Usable) {
                    throw new NotSupportedException("Block unusable");
                }

                if (sliceOffset < Offset) {
                    throw new IndexOutOfRangeException();
                }

                if (sliceOffset + sliceLength > Offset + Length) {
                    throw new IndexOutOfRangeException();
                }

                return new ArraySegment<T>(Buffer, sliceOffset, sliceLength);
            }

            public override int GetHashCode() {
                var hash = 17;

                unchecked {
                    hash = hash * 23 + Buffer.GetHashCode();
                    hash = hash * 23 + Offset.GetHashCode();
                    hash = hash * 23 + Stride.GetHashCode();
                }

                return hash;
            }

            public override bool Equals(object obj) {
                if (obj is Fragment other) {
                    return Equals(other);
                }

                return base.Equals(obj);
            }

            public bool Equals(Fragment other) {
                return Buffer.Equals(other.Buffer)
                    && Offset.Equals(other.Offset)
                    && Length.Equals(other.Length);
            }

            public void Dispose() {
                Length = 0;
            }

            public IFragment<T> Prepare(int size) {
                Debug.Assert(size > 0);

                Length = size;

                return this;
            }

            public override string ToString() {
                return $"Length: {Length} ({Stride})";
            }
        }

        public static readonly IFragment<T>
            EmptyFragment = new Fragment(new T[0], 0, 0);

        private readonly List<Fragment>
            fragments = new List<Fragment>();
        private readonly object 
            syncLock = new object();

        public StrideBuffer(int strideChunk = 1024) {
            if (strideChunk < 4) {
                throw new ArgumentException($"value < 4", nameof(strideChunk));
            }

            StrideChunk = strideChunk;
        }

        public int StrideChunk {
            get;
        }

        public bool Collect(IFragment<T> fragment)
        {
            lock (syncLock) {
                if (fragment is Fragment fragmentOfFragment) {
                    return fragments.Remove(fragmentOfFragment);
                }
            }

            return false;
        }

        public void Prepare(int size, int count) {
            var stride = Stride(size);
            var length = stride * count;

            var buffer = new T[length];

            lock (syncLock) {
                for (var offset = 0; offset < length; offset += stride) {
                    CreateFragment(buffer, offset, stride);
                }
            }
        }

        public IFragment<T> GetFragment(int size) {
            var stride = Stride(size);

            lock (syncLock) {
                if (TryGetFragment(stride, out var fragment) == false) {
                    var buffer = new T[stride];

                    fragment = CreateFragment(buffer, 0, stride);
                }

                return fragment.Prepare(size);
            }
        }

        private bool TryGetFragment(int stride, out Fragment segment) {
            var iterator = fragments.GetEnumerator();

            while (iterator.MoveNext()) {
                segment = iterator.Current;

                var segmentStride = segment.Stride;
                if (segmentStride == stride && segment.Usable) {
                    return true;
                }

                if (segmentStride > stride) {
                    return false;
                }
            }

            segment = null;

            return false;
        }

        private Fragment CreateFragment(T[] buffer, int offset, int stride) {
            var index = 0;
            var segment = new Fragment(buffer, offset, stride);

            for (; index < fragments.Count && stride > fragments[index].Stride; ++index) {
                
            }

            fragments.Insert(index, segment);

            return segment;
        }

        private int Stride(int size) {
            var mod = size % StrideChunk;

            if (mod > 0) {
                return size + StrideChunk - mod;
            }
            else {
                return size;
            }
        }
    }
}
