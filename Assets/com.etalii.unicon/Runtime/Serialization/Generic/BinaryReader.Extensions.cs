// Copyright (c) Peter Vrenken. All rights reserved. 

// ReSharper disable MemberCanBePrivate.Global

namespace EtAlii.Unicon
{
    using System;
    using System.IO;
    using System.Reflection;
    using UnityEngine;

    /// <summary>
    /// Simple extension class to make flat, layman's serialization fun and performant.
    /// </summary>
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Read a Vector2 instance from the stream using the BinaryReader.
        /// </summary>
        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            return new Vector2(x, y);
        }

        /// <summary>
        /// Read a Vector3 instance from the stream using the BinaryReader.
        /// </summary>
        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Read a Vector4 instance from the stream using the BinaryReader.
        /// </summary>
        public static Vector3 ReadVector4(this BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var w = reader.ReadSingle();
            return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Read a Rect instance from the stream using the BinaryReader.
        /// </summary>
        public static Rect ReadRect(this BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var w = reader.ReadSingle();
            var h = reader.ReadSingle();
            return new Rect(x, y, w, h);
        }
        
        /// <summary>
        /// Read a Quaternion instance from the stream using the BinaryReader.
        /// </summary>
        public static Quaternion ReadQuaternion(this BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var w = reader.ReadSingle();
            return new Quaternion(x, y, z, w);
        }
        
        /// <summary>
        /// Read a Color instance from the stream using the BinaryReader.
        /// </summary>
        public static Color ReadColor(this BinaryReader reader)
        {
            var r = reader.ReadSingle();
            var g = reader.ReadSingle();
            var b = reader.ReadSingle();
            var a = reader.ReadSingle();
            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Read a Bounds instance from the stream using the BinaryReader.
        /// </summary>
        public static Bounds ReadBounds(this BinaryReader reader)
        {
            var center = reader.ReadVector3();
            var size = reader.ReadVector3();
            return new Bounds(center, size);
        }

        /// <summary>
        /// Read a Matrix4x4 instance from the stream using the BinaryReader.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static Matrix4x4 ReadMatrix4x4(this BinaryReader reader)
        {
            var column0 = reader.ReadVector4();
            var column1 = reader.ReadVector4();
            var column2 = reader.ReadVector4();
            var column3 = reader.ReadVector4();
            return new Matrix4x4(column0, column1, column2, column3);
        }

        /// <summary>
        /// Read a typed instance from the stream using the BinaryReader.
        /// <remarks>Consider using the alternative generic methods instead as reading untyped data is pretty slow (and also might take up mare data).</remarks>
        /// </summary>
        /// <exception cref="NotSupportedException">Gets thrown when the method is not able to read instances of the specified type</exception>
        public static object ReadTyped(this BinaryReader reader)
        {
            var typeId = (TypeId)reader.ReadByte();

            return typeId switch
            {
                TypeId.String => reader.ReadString(),
                TypeId.Char => reader.ReadChar(),
                TypeId.Boolean => reader.ReadBoolean(),
                TypeId.SByte => reader.ReadSByte(),
                TypeId.Byte => reader.ReadByte(),
                TypeId.Int16 => reader.ReadInt16(),
                TypeId.Int32 => reader.ReadInt32(),
                TypeId.Int64 => reader.ReadInt64(),
                TypeId.UInt16 => reader.ReadUInt16(),
                TypeId.UInt32 => reader.ReadUInt32(),
                TypeId.UInt64 => reader.ReadUInt64(),
                TypeId.Single => reader.ReadSingle(),
                TypeId.Double => reader.ReadDouble(),
                TypeId.Decimal => reader.ReadDecimal(),
                TypeId.DateTime => reader.ReadDateTime(),
                TypeId.DateTimeOffset => reader.ReadDateTimeOffset(),
                TypeId.TimeSpan => reader.ReadTimeSpan(),
                TypeId.Guid => reader.ReadGuid(),
                TypeId.Version => reader.ReadVersion(),
                TypeId.Range => reader.ReadRange(),
                
                TypeId.Vector2 => reader.ReadVector2(),
                TypeId.Vector3 => reader.ReadVector3(),
                TypeId.Vector4 => reader.ReadVector4(),
                TypeId.Quaternion => reader.ReadQuaternion(),
                TypeId.Rect => reader.ReadRect(),
                TypeId.Color => reader.ReadColor(),

                TypeId.Bounds => reader.ReadBounds(),
                TypeId.Matrix4x4 => reader.ReadMatrix4x4(),

                TypeId.None => null,
                _ => throw new NotSupportedException($"TypeId is not supported: {typeId}")
            };

        }

#pragma warning disable S4144 // Disable Sonarqube warning "Methods should not have identical implementations"
// Reason is that we really want two generic methods, one for processing classes and one for processing structs.
#nullable enable
        /// <summary>
        /// Read an optional (nullable) reference type from the stream using the BinaryReader.
        /// </summary>
        /// <remarks>This method will first read bool prefix from the stream to indicate if the variable was unassigned (null) or not.</remarks>
        public static T? ReadOptionalReferenceType<T>(this BinaryReader reader)
            where T : class
        {
            var hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                return Read<T>(reader);
            }

            return null;
        }

        /// <summary>
        /// Read an optional (nullable) value type from the stream using the BinaryReader.
        /// </summary>
        /// <remarks>This method will first read bool prefix from the stream to indicate if the variable was unassigned (null) or not.</remarks>
        public static T? ReadOptionalValueType<T>(this BinaryReader reader)
            where T : struct
        {
            var hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                return Read<T>(reader);
            }

            return null;
        }

#nullable disable
#pragma warning restore S4144
        /// <summary>
        /// Read a generic instance from the stream using the BinaryReader.
        /// </summary>
        /// <exception cref="NotSupportedException">Gets thrown when the method is not able to read instances of the specified type</exception>
        public static T Read<T>(this BinaryReader reader)
        {
            var type = typeof(T);
            if (typeof(IBinarySerializable).IsAssignableFrom(type))
            {
                IBinarySerializable serializable;
                if (type.IsClass)
                {
#pragma warning disable S3011
                    // reasoning We explicitly want to get access to any constructor, either a public or a non public one.
                    var constructor =
                        type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                            Array.Empty<Type>(), Array.Empty<ParameterModifier>());
#pragma warning restore S3011

                    serializable = (IBinarySerializable)constructor!.Invoke(Array.Empty<object>());
                }
                else
                {
                    serializable = (IBinarySerializable)Activator.CreateInstance<T>();
                }

                serializable.Read(reader);
                return (T)serializable;
            }

            object result = typeof(T) switch
            {
                _ when typeof(T) == typeof(string) => reader.ReadString(),
                _ when typeof(T) == typeof(int) => reader.ReadInt32(),
                _ when typeof(T) == typeof(uint) => reader.ReadUInt32(),
                _ when typeof(T) == typeof(short) => reader.ReadInt16(),
                _ when typeof(T) == typeof(ushort) => reader.ReadUInt16(),
                _ when typeof(T) == typeof(long) => reader.ReadInt64(),
                _ when typeof(T) == typeof(ulong) => reader.ReadUInt64(),
                _ when typeof(T) == typeof(char) => reader.ReadChar(),
                _ when typeof(T) == typeof(byte) => reader.ReadByte(),
                _ when typeof(T) == typeof(sbyte) => reader.ReadSByte(),
                _ when typeof(T) == typeof(float) => reader.ReadSingle(),
                _ when typeof(T) == typeof(double) => reader.ReadDouble(),
                _ when typeof(T) == typeof(Guid) => reader.ReadGuid(),
                _ when typeof(T) == typeof(bool) => reader.ReadBoolean(),
                _ when typeof(T) == typeof(decimal) => reader.ReadDecimal(),
                _ when typeof(T) == typeof(Range) => reader.ReadRange(),
                _ when typeof(T) == typeof(DateTime) => reader.ReadDateTime(),
                _ when typeof(T) == typeof(DateTimeOffset) => reader.ReadDateTimeOffset(),
                _ when typeof(T) == typeof(TimeSpan) => reader.ReadTimeSpan(),
                _ when typeof(T) == typeof(Version) => reader.ReadVersion(),
                _ when typeof(T) == typeof(Vector2) => reader.ReadVector2(),
                _ when typeof(T) == typeof(Vector3) => reader.ReadVector3(),
                _ when typeof(T) == typeof(Vector4) => reader.ReadVector4(),
                _ when typeof(T) == typeof(Quaternion) => reader.ReadQuaternion(),
                _ when typeof(T) == typeof(Rect) => reader.ReadRect(),
                _ when typeof(T) == typeof(Color) => reader.ReadColor(),

                _ when typeof(T) == typeof(Bounds) => reader.ReadBounds(),
                _ when typeof(T) == typeof(Matrix4x4) => reader.ReadMatrix4x4(),
                
                _ => throw new ArgumentOutOfRangeException($"Unable to read {typeof(T)} from BinaryReader")
            };
            return (T)result;
        }

        /// <summary>
        /// Delegate reading from the stream to the assigned read function.
        /// </summary>
        public static TSerializable Read<TSerializable>(this BinaryReader reader,
            Func<BinaryReader, TSerializable> read)
        {
            return read(reader);
        }

        /// <summary>
        /// Read a set of values to the stream using the BinaryReader.
        /// This method will first read a prefix int that identifies how many items should be read.
        /// </summary>
        public static T[] ReadMany<T>(this BinaryReader reader)
        {
            var count = reader.ReadInt32();
            var result = new T[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = Read<T>(reader);
            }

            return result;
        }

        /// <summary>
        /// Read a Range instance from the stream using the BinaryReader.
        /// </summary>
        public static Range ReadRange(this BinaryReader reader)
        {
            return new Range(reader.ReadInt32(), reader.ReadInt32());
        }

        /// <summary>
        /// Read a Guid instance from the stream using the BinaryReader.
        /// </summary>
        public static Guid ReadGuid(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(16);
            return new Guid(bytes);
        }

        /// <summary>
        /// Read a DateTime instance from the stream using the BinaryReader.
        /// </summary>
        public static DateTime ReadDateTime(this BinaryReader reader)
        {
            var kind = (DateTimeKind)reader.ReadByte();
            var ticks = reader.ReadInt64();
            return new DateTime(ticks, kind);
        }
        
        /// <summary>
        /// Read a DateTime instance from the stream using the BinaryReader.
        /// </summary>
        public static DateTimeOffset ReadDateTimeOffset(this BinaryReader reader)
        {
            var ticks = reader.ReadInt64();
            var offset = reader.ReadTimeSpan();
            return new DateTimeOffset(ticks, offset);
        }

        /// <summary>
        /// Read a TimeSpan instance from the stream using the BinaryReader.
        /// </summary>
        public static TimeSpan ReadTimeSpan(this BinaryReader reader)
        {
            var ticks = reader.ReadInt64();
            return new TimeSpan(ticks);
        }

        /// <summary>
        /// Read a Version instance from the stream using the BinaryReader.
        /// </summary>
        public static Version ReadVersion(this BinaryReader reader)
        {
            var major = reader.ReadInt32();
            var minor = reader.ReadInt32();
            var build = reader.ReadInt32();
            var revision = reader.ReadInt32();
            return new Version(major, minor, build, revision);
        }
    }
}