// Copyright (c) Peter Vrenken. All rights reserved. 

namespace EtAlii.Unicon
{
    using System.IO;

    public static partial class Binary
    {
        /// <summary>
        /// Deserialize a TData instance from the provided BinaryReader. 
        /// </summary>
        public static TData Deserialize<TData>(BinaryReader reader)
            where TData : IBinarySerializable, new()
        {
            return reader.Read<TData>();
        }
        
        /// <summary>
        /// Deserialize a TData instance from the provided Stream. 
        /// </summary>
        public static TData Deserialize<TData>(Stream stream)
            where TData : IBinarySerializable, new()
        {
            using var reader = new BinaryReader(stream);
            return reader.Read<TData>();
        }
        
        /// <summary>
        /// Deserialize a TData instance from the provided byte array. 
        /// </summary>
        public static TData Deserialize<TData>(byte[] serializedData)
            where TData : IBinarySerializable, new()
        {
            using var stream = new MemoryStream(serializedData);
            using var reader = new BinaryReader(stream);
            return reader.Read<TData>();
        }
        
        /// <summary>
        /// Deserialize multiple TData instances from the provided BinaryReader. 
        /// </summary>
        public static TData[] DeserializeMany<TData>(BinaryReader reader)
            where TData : IBinarySerializable, new()
        {
            return reader.ReadMany<TData>();
        }
        
        /// <summary>
        /// Deserialize multiple TData instances from the provided stream. 
        /// </summary>
        public static TData[] DeserializeMany<TData>(Stream stream)
            where TData : IBinarySerializable, new()
        {
            using var reader = new BinaryReader(stream);
            return reader.ReadMany<TData>();
        }
        
        /// <summary>
        /// Deserialize multiple TData instances from the provided byte array. 
        /// </summary>
        public static TData[] DeserializeMany<TData>(byte[] serializedData)
            where TData : IBinarySerializable, new()
        {
            using var stream = new MemoryStream(serializedData);
            using var reader = new BinaryReader(stream);
            return reader.ReadMany<TData>();
        }

    }
}