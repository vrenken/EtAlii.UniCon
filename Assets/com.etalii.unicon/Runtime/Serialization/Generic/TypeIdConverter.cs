// Copyright (c) Peter Vrenken. All rights reserved. 

namespace EtAlii.Unicon
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Converter class that is able to convert between Type and Typeid. It becomes useful in more complex patterns.
    /// </summary>
    public static class TypeIdConverter
    {
        /// <summary>
        /// Determine the Type from the provided TypeId. 
        /// </summary>
        public static Type ToType(TypeId typeId)
        {
            return typeId switch
            {
                TypeId.String => typeof(string),
                TypeId.Char => typeof(char),
                TypeId.Boolean => typeof(bool),
                TypeId.SByte => typeof(sbyte),
                TypeId.Byte => typeof(byte),
                TypeId.Int16 => typeof(short),
                TypeId.Int32 => typeof(int),
                TypeId.Int64 => typeof(long),
                TypeId.UInt16 => typeof(ushort),
                TypeId.UInt32 => typeof(uint),
                TypeId.UInt64 => typeof(ulong),
                TypeId.Single => typeof(float),
                TypeId.Double => typeof(double),
                TypeId.Decimal => typeof(decimal),
                TypeId.DateTime => typeof(DateTime),
                TypeId.DateTimeOffset => typeof(DateTimeOffset),
                TypeId.TimeSpan => typeof(TimeSpan),
                TypeId.Guid => typeof(Guid),
                TypeId.Version => typeof(Version),
                TypeId.Range => typeof(Range),
                
                TypeId.Vector2 => typeof(Vector2),
                TypeId.Vector3 => typeof(Vector3),
                TypeId.Vector4 => typeof(Vector4),
                TypeId.Quaternion => typeof(Quaternion),
                TypeId.Rect => typeof(Rect),
                TypeId.Color => typeof(Color),

                TypeId.Bounds => typeof(Bounds),
                TypeId.Matrix4x4 => typeof(Matrix4x4),

                TypeId.None => null,
                _ => throw new NotSupportedException($"TypeId is not supported: {typeId}")
            };
        }

        /// <summary>
        /// Determine the TypeId from the provided object. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static TypeId ToTypeId(object value)
        {
            if (value is null) return TypeId.None;
            
            if (value.GetType().IsEnum) return TypeId.Int32;

            return value switch
            {
                string => TypeId.String,
                char => TypeId.Char,
                bool => TypeId.Boolean,
                sbyte => TypeId.SByte,
                byte => TypeId.Byte,
                short => TypeId.Int16,
                int => TypeId.Int32,
                long => TypeId.Int64,
                ushort => TypeId.UInt16,
                uint => TypeId.UInt32,
                ulong => TypeId.UInt64,
                float => TypeId.Single,
                double => TypeId.Double,
                decimal => TypeId.Decimal,
                DateTime => TypeId.DateTime,
                DateTimeOffset => TypeId.DateTimeOffset,
                TimeSpan => TypeId.TimeSpan,
                Guid => TypeId.Guid,
                Version => TypeId.Version,
                Range => TypeId.Range,
                
                Vector2 => TypeId.Vector2,
                Vector3 => TypeId.Vector3,
                Vector4 => TypeId.Vector4,
                Quaternion => TypeId.Quaternion,
                Rect => TypeId.Rect,
                Color => TypeId.Color,
                
                Bounds => TypeId.Bounds,
                Matrix4x4 => TypeId.Matrix4x4,
                
                _ => throw new NotSupportedException("Type is not supported: " + value.GetType().Name)
            };
        }
    }
}
