// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.Ubigia

namespace EtAlii.UniCon
{
    using System;

    /// <summary>
    /// A guid can be made more compact when the full alphabet is used.
    /// </summary>
    public static class ShortGuid
    {
        /// <summary>
        /// Create a compact ShortGuid string.
        /// This method takes a Guid and compacts it using Base36 encoding.
        /// </summary>
        /// <returns></returns>
        public static string New()
        {
            return Base36Convert.ToString(Guid.NewGuid()).ToUpper();
        }
    }
}
