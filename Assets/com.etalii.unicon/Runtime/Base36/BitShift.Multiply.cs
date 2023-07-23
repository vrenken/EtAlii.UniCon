// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.Ubigia


namespace EtAlii.UniCon
{
    using System;

    internal static partial class BitShift
    {
        public static void Multiply(ref Span<bool> target, bool[] multiplication)
        {
            Span<bool> original = new bool[target.Length];
            target.CopyTo(original);

            target = Array.Empty<bool>();

            var bitsToIterate = multiplication.Length;
            for (var i = bitsToIterate - 1; i >= 0; i--)
            {
                if (multiplication[i])
                {
                    Add(ref target, original);
                }

                Span<bool> newOriginal = new bool[original.Length + 1];
                original.CopyTo(newOriginal);
                original = newOriginal;
            }
        }
    }
}