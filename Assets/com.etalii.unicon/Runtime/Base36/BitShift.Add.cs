// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.Ubigia


namespace EtAlii.UniCon
{
    using System;
    using System.Runtime.CompilerServices;

    internal static partial class BitShift
    {
        /// <summary>
        /// Bitwise-add the addition on top of the target span.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="addition"></param>
        public static void Add(ref Span<bool> target, ReadOnlySpan<bool> addition)
        {
            var carry = target.Length < addition.Length
                ? AddAdditionToSmallerTarget(ref target, addition)
                : AddAdditionToLargerTarget(ref target, addition);

            while (carry > 0)
            {
                Span<bool> newTarget = new bool[target.Length + 1];
                target.CopyTo(newTarget[1..]);
                target = newTarget;
                target[0] = true;
                carry >>= 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long AddAdditionToSmallerTarget(ref Span<bool> target, ReadOnlySpan<bool> addition)
        {
            long carry = 0;

            var bitsToIterate = target.Length;
            var delta = addition.Length - bitsToIterate;
            for (var i = bitsToIterate - 1; i >= 0; i--)
            {
                carry = carry + (target[i] ? 1 : 0) + (addition[i + delta] ? 1 : 0);
                target[i] = (carry & 0x1) == 0x1;
                carry >>= 1;
            }

            bitsToIterate = delta;
            for (var i = bitsToIterate - 1; i >= 0; i--)
            {
                carry = carry + (addition[i] ? 1 : 0);
                Span<bool> newTarget = new bool[target.Length + 1];
                target.CopyTo(newTarget[1..]);
                target = newTarget;
                target[0] = (carry & 0x1) == 0x1;
                carry >>= 1;
            }

            return carry;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long AddAdditionToLargerTarget(ref Span<bool> target, ReadOnlySpan<bool> addition)
        {
            long carry = 0;

            var bitsToIterate = addition.Length;
            var delta = target.Length - bitsToIterate;
            for (var i = bitsToIterate - 1; i >= 0; i--)
            {
                carry = carry + (target[i + delta] ? 1 : 0) + (addition[i] ? 1 : 0);
                target[i + delta] = (carry & 0x1) == 0x1;
                carry >>= 1;
            }

            bitsToIterate = delta;
            for (var i = bitsToIterate - 1; i >= 0; i--)
            {
                carry = carry + (target[i] ? 1 : 0);
                target[i] = (carry & 0x1) == 0x1;
                carry >>= 1;
            }

            return carry;
        }
    }
}