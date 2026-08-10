/*
 * OpenCAGE fork addition.
 *
 * Upstream DotRecast is authored against net8.0. This fork also has to compile as netstandard2.0
 * so CathodeLib can be consumed from .NET Framework, and these shims supply the BCL types that
 * netstandard2.0 does not ship. They compile away entirely on netstandard2.1+ / .NET Core, so the
 * net8.0 build is untouched.
 *
 * Call sites needing a *static member* on an existing BCL type (Math.Clamp, Array.Fill,
 * float.IsFinite, MemoryMarshal.CreateSpan, HashCode.Combine) cannot be shimmed this way, and are
 * instead routed through RcMath / RcArrays / RcSpans / RcHashCodes.
 */

#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP

namespace System
{
    internal static class MathF
    {
        public const float PI = 3.14159265358979323846f;
        public const float E = 2.7182818284590452354f;

        public static float Abs(float x) => Math.Abs(x);
        public static float Sqrt(float x) => (float)Math.Sqrt(x);
        public static float Floor(float x) => (float)Math.Floor(x);
        public static float Ceiling(float x) => (float)Math.Ceiling(x);
        public static float Round(float x) => (float)Math.Round(x);
        public static float Round(float x, int digits) => (float)Math.Round(x, digits);
        public static float Truncate(float x) => (float)Math.Truncate(x);
        public static float Sin(float x) => (float)Math.Sin(x);
        public static float Cos(float x) => (float)Math.Cos(x);
        public static float Tan(float x) => (float)Math.Tan(x);
        public static float Sinh(float x) => (float)Math.Sinh(x);
        public static float Cosh(float x) => (float)Math.Cosh(x);
        public static float Tanh(float x) => (float)Math.Tanh(x);
        public static float Asin(float x) => (float)Math.Asin(x);
        public static float Acos(float x) => (float)Math.Acos(x);
        public static float Atan(float x) => (float)Math.Atan(x);
        public static float Atan2(float y, float x) => (float)Math.Atan2(y, x);
        public static float Pow(float x, float y) => (float)Math.Pow(x, y);
        public static float Exp(float x) => (float)Math.Exp(x);
        public static float Log(float x) => (float)Math.Log(x);
        public static float Log(float x, float newBase) => (float)Math.Log(x, newBase);
        public static float Log2(float x) => (float)Math.Log(x, 2.0d);
        public static float Log10(float x) => (float)Math.Log10(x);
        public static float Min(float x, float y) => Math.Min(x, y);
        public static float Max(float x, float y) => Math.Max(x, y);
        public static int Sign(float x) => Math.Sign(x);
        public static float IEEERemainder(float x, float y) => (float)Math.IEEERemainder(x, y);
    }

    /// <summary>Supports the C# 8 index-from-end operator (^) on netstandard2.0.</summary>
    internal readonly struct Index : IEquatable<Index>
    {
        private readonly int _value;

        public Index(int value, bool fromEnd = false)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Non-negative number required.");
            _value = fromEnd ? ~value : value;
        }

        private Index(int value) => _value = value;

        public static Index Start => new Index(0);
        public static Index End => new Index(~0);

        public static Index FromStart(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Non-negative number required.");
            return new Index(value);
        }

        public static Index FromEnd(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Non-negative number required.");
            return new Index(~value);
        }

        public int Value => _value < 0 ? ~_value : _value;
        public bool IsFromEnd => _value < 0;

        public int GetOffset(int length) => _value < 0 ? _value + length + 1 : _value;

        public override bool Equals(object value) => value is Index other && _value == other._value;
        public bool Equals(Index other) => _value == other._value;
        public override int GetHashCode() => _value;
        public override string ToString() => IsFromEnd ? "^" + ((uint)Value).ToString() : ((uint)Value).ToString();

        public static implicit operator Index(int value) => FromStart(value);
    }

    /// <summary>Supports the C# 8 range operator (..) on netstandard2.0.</summary>
    internal readonly struct Range : IEquatable<Range>
    {
        public Index Start { get; }
        public Index End { get; }

        public Range(Index start, Index end)
        {
            Start = start;
            End = end;
        }

        public static Range StartAt(Index start) => new Range(start, Index.End);
        public static Range EndAt(Index end) => new Range(Index.Start, end);
        public static Range All => new Range(Index.Start, Index.End);

        public (int Offset, int Length) GetOffsetAndLength(int length)
        {
            int start = Start.GetOffset(length);
            int end = End.GetOffset(length);
            if ((uint)end > (uint)length || (uint)start > (uint)end)
                throw new ArgumentOutOfRangeException(nameof(length));
            return (start, end - start);
        }

        public override bool Equals(object value) => value is Range r && r.Start.Equals(Start) && r.End.Equals(End);
        public bool Equals(Range other) => other.Start.Equals(Start) && other.End.Equals(End);
        public override int GetHashCode() => (Start.GetHashCode() * 31) + End.GetHashCode();
        public override string ToString() => Start.ToString() + ".." + End.ToString();
    }
}

namespace System.Runtime.CompilerServices
{
    /// <summary>Emitted by the compiler for <c>array[range]</c> expressions.</summary>
    internal static class RuntimeHelpersEx
    {
        public static T[] GetSubArray<T>(T[] array, Range range)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            (int offset, int length) = range.GetOffsetAndLength(array.Length);
            if (length == 0)
                return new T[0];

            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
    }
}

#endif
