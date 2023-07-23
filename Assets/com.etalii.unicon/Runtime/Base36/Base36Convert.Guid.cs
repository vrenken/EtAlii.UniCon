// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.Ubigia

namespace EtAlii.UniCon
{
    using System;

    public static partial class Base36Convert
    {
        /// <summary>
        /// Convert the given Guid into a Base36 encoded string.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToString(Guid guid)
        {
            var bytes = guid.ToByteArray();
            return ToString(bytes);
        }

        /// <summary>
        /// Convert the given Base36 encoded string into a Guid.
        /// </summary>
        /// <param name="base36String"></param>
        /// <returns></returns>
        public static Guid ToGuid(string base36String)
        {
            var bytes = ToBytes(base36String);
            return new Guid(bytes);
        }
    }
}