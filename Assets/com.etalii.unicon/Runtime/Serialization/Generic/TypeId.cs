// Copyright (c) Peter Vrenken. All rights reserved. 

namespace EtAlii.Unicon
{
    /// <summary>
    /// A TypeId enumeration with which to write undefined (non-IBinarySerializable) object state.
    /// This TypeId us used in the <see cref="BinaryWriterExtensions.WriteTyped"/> and <see cref="BinaryReaderExtensions.ReadTyped"/> extensions.
    /// It is wise to use these methods with care as most often it is best to know what should be serialized.
    /// </summary>
    public enum TypeId : byte
    {
        None = 0,
        String,
        Char,
        Boolean,
        SByte,
        Byte,
        Int16,
        Int32,
        Int64,
        UInt16,
        UInt32,
        UInt64,
        Single,
        Double,
        Decimal,
        DateTime,
        DateTimeOffset,
        TimeSpan,
        Guid,
        Version,
        Range,
        Vector2,
        Vector3,
        Vector4,
        Quaternion,
        Rect,
        Color,
        
        Bounds,
        // ReSharper disable once InconsistentNaming
        Matrix4x4,
    }
}