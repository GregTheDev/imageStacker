﻿using imageStacker.Core.Abstraction;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace imageStacker.Core.ByteImage.Filters
{
    public class MinVecFilter : IFilter<MutableByteImage>
    {
        public MinVecFilter(IMinFilterOptions options = null)
        {
            this.Name = options?.Name ?? nameof(MinVecFilter);
        }

        public string Name { get; }

        public bool IsSupported => Sse2.IsSupported;

        public unsafe void Process(MutableByteImage currentPicture, MutableByteImage nextPicture)
        {
            int simdSize = Vector128<byte>.Count;
            int length = currentPicture.Data.Length;

            fixed (byte* dataPtr1 = currentPicture.Data)
            fixed (byte* dataPtr2 = nextPicture.Data)
            {
                int i = 0;
                for (; i < length; i += simdSize)
                {
                    Sse2.Store(dataPtr1 + i, Sse2.Min(
                        Sse2.LoadVector128(dataPtr1 + i),
                        Sse2.LoadVector128(dataPtr2 + i)));
                }

                for (; i < length; i++)
                {
                    var nextData = *(dataPtr2 + i);
                    if (*(dataPtr1 + i) < nextData)
                    {
                        *(dataPtr1 + i) = nextData;
                    }
                }
            }
        }
    }
}
