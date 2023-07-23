// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.Ubigia


namespace EtAlii.UniCon
{
    using System.Collections;
    using System.Collections.Generic;

    internal static partial class BitShift
    {
        public static byte[] Divide(byte[] bytes, ulong divisor, out ulong remainder, bool preserveSize = true)
        {
            //the byte array MUST be little-endian here or the operation will be totally fubared.
            var bitArray = new BitArray(bytes);

            ulong buffer = 0;
            byte quotientBuffer = 0;
            byte qBufferLen = 0;
            var quotient = new List<byte>();

            //the bitarray indexes its values in little-endian fashion.
            //as the index increases we move from LSB to MSB.
            for (var i = bitArray.Length - 1; i >= 0; --i)
            {
                //The basic idea is similar to decimal long division.
                //starting from the most significant bit, take enough bits
                //to form a number divisible by (greater than) the divisor.
                buffer = (buffer << 1) + (ulong)(bitArray[i] ? 1 : 0);
                if (buffer >= divisor)
                {
                    //Now divide; buffer will never be >= divisor * 2,
                    //so the quotient of buffer / divisor is always 1...
                    quotientBuffer = (byte)((quotientBuffer << 1) + 1);
                    //then subtract the divisor from the buffer,
                    //to produce the remainder to be carried forward.
                    buffer -= divisor;
                }
                else
                {
                    //to keep our place; if buffer < divisor,
                    //then by definition buffer / divisor == 0 R buffer.
                    quotientBuffer = (byte)(quotientBuffer << 1);
                }

                qBufferLen++;

                if (qBufferLen == 8)
                {
                    //preserveSize forces the output array to be the same number of bytes as the input.
                    //otherwise, insert only if we're inserting a nonzero byte or have already done so,
                    //to truncate leading zeroes.
                    if (preserveSize || quotient.Count > 0 || quotientBuffer > 0)
                    {
                        quotient.Insert(0, quotientBuffer);
                    }

                    //reset the buffer
                    quotientBuffer = 0;
                    qBufferLen = 0;
                }
            }

            //and when all is said and done what's left in our buffer is the remainder.
            remainder = buffer;

            return quotient.ToArray();
        }
    }
}
