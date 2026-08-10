using System.Runtime.CompilerServices;

namespace DotRecast.Core
{
    public static class RcHashCodes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }

        // System.HashCode is netstandard2.1+; these keep the sources buildable for netstandard2.0
        // consumers (CathodeLib is referenced from .NET Framework) without an extra package.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2>(T1 v1, T2 v2)
        {
            return CombineHashCodes(v1?.GetHashCode() ?? 0, v2?.GetHashCode() ?? 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3>(T1 v1, T2 v2, T3 v3)
        {
            return CombineHashCodes(Combine(v1, v2), v3?.GetHashCode() ?? 0);
        }

        // From Thomas Wang, https://gist.github.com/badboy/6267743
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint WangHash(uint a)
        {
            a = (~a) + (a << 18); // a = (a << 18) - a - 1;
            a = a ^ (a >> 31);
            a = a * 21; // a = (a + (a << 2)) + (a << 4);
            a = a ^ (a >> 11);
            a = a + (a << 6);
            a = a ^ (a >> 22);
            return (uint)a;
        }
    }
}