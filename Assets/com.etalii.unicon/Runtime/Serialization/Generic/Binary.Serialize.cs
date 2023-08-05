// Copyright (c) Peter Vrenken. All rights reserved. 

using System.Collections.Generic;

namespace EtAlii.Unicon
{
    using System.IO;

    public static partial class Binary
    {
        /// <summary>
        /// Serialize a TData instance to the provided BinaryWriter. 
        /// </summary>
        public static void Serialize(IBinarySerializable item, BinaryWriter writer)
        {
            writer.Write(item);
            writer.Flush();
        }
        
        /// <summary>
        /// Serialize a TData instance to the provided stream. 
        /// </summary>
        public static void Serialize(IBinarySerializable item, Stream stream)
        {
            using var writer = new BinaryWriter(stream);
            writer.Write(item);
            writer.Flush();
        }
        
        /// <summary>
        /// Serialize a TData instance as a byte array. 
        /// </summary>
        public static byte[] Serialize(IBinarySerializable item)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.Write(item);
            writer.Flush();
            return stream.ToArray();
        }
        
        /// <summary>
        /// Serialize multiple TData instances to the provided BinaryWriter. 
        /// </summary>
        public static void SerializeMany<TData>(IReadOnlyList<TData> items, BinaryWriter writer)
        {
            writer.WriteMany(items);
            writer.Flush();
        }
        
        /// <summary>
        /// Serialize multiple TData instances to the provided stream. 
        /// </summary>
        public static void SerializeMany(IReadOnlyList<IBinarySerializable> items, Stream stream)
        {
            using var writer = new BinaryWriter(stream);
            writer.WriteMany(items);
            writer.Flush();
        }
        
        /// <summary>
        /// Serialize multiple TData instances as a byte array. 
        /// </summary>
        public static byte[] SerializeMany(IReadOnlyList<IBinarySerializable> items)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.WriteMany(items);
            writer.Flush();
            return stream.ToArray();
        }

    }
}