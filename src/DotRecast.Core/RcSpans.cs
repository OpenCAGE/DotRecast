using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DotRecast.Core
{
    public static class RcSpans
    {
        // MemoryMarshal.CreateSpan is netstandard2.1+; this keeps the sources buildable
        // for netstandard2.0 consumers (CathodeLib is referenced from .NET Framework).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Span<T> CreateSpan<T>(scoped ref T reference, int length) where T : unmanaged
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
            return MemoryMarshal.CreateSpan(ref reference, length);
#else
            return new Span<T>(Unsafe.AsPointer(ref reference), length);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy<T>(ReadOnlySpan<T> src, Span<T> dst)
        {
            src.CopyTo(dst);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy<T>(ReadOnlySpan<T> src, int srcIdx, Span<T> dst, int dstIdx, int length)
        {
            var slicedSrc = src.Slice(srcIdx, length);
            var slicedDst = dst.Slice(dstIdx);
            slicedSrc.CopyTo(slicedDst);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Move<T>(Span<T> src, int srcIdx, int dstIdx, int length)
        {
            var slicedSrc = src.Slice(srcIdx, length);
            var slicedDst = src.Slice(dstIdx, length);
            slicedSrc.CopyTo(slicedDst);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fill<T>(Span<T> span, T value, int start, int count)
        {
            span.Slice(start, count).Fill(value);
        }
    }
}