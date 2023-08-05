// Copyright (c) Peter Vrenken. All rights reserved. 

namespace EtAlii.Unicon
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    public static partial class LogEntrySerialization
    {
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static void SerializeException(Exception exception, BinaryWriter writer)
        {
            // TODO: Implement.
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static Exception DeserializeException(BinaryReader reader)
        {
            // TODO: Implement.
            return null;
        }
    }
}