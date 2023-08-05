// Copyright (c) Peter Vrenken. All rights reserved. 

// ReSharper disable MemberCanBePrivate.Global

namespace EtAlii.Unicon
{
    using System;
    using System.IO;
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Simple extension class to make flat, layman's serialization fun and performant.
    /// </summary>
    public static class BinaryWriterExtensions
    {
        /// <summary>
        /// Write the Vector2 instance to the stream using the BinaryWriter.
        /// </summary>
        public static void Write(this BinaryWriter writer, Vector2 vector2)
        {
            writer.Write(vector2.x);
            writer.Write(vector2.y);
        }

        /// <summary>
        /// Write the Vector3 instance to the stream using the BinaryWriter.
        /// </summary>
        public static void Write(this BinaryWriter writer, Vector3 vector3)
        {
            writer.Write(vector3.x);
            writer.Write(vector3.y);
            writer.Write(vector3.z);
        }

        /// <summary>
        /// Write the Vector4 instance to the stream using the BinaryWriter.
        /// </summary>
        public static void Write(this BinaryWriter writer, Vector4 vector3)
        {
            writer.Write(vector3.x);
            writer.Write(vector3.y);
            writer.Write(vector3.z);
            writer.Write(vector3.w);
        }

        /// <summary>
        /// Write the Rect instance to the stream using the BinaryWriter.
        /// </summary>
        public static void Write(this BinaryWriter writer, Rect rect)
        {
            writer.Write(rect.x);
            writer.Write(rect.y);
            writer.Write(rect.width);
            writer.Write(rect.height);
        }

        /// <summary>
        /// Write the Quaternion instance to the stream using the BinaryWriter.
        /// </summary>
        public static void Write(this BinaryWriter writer, Quaternion quaternion)
        {
            writer.Write(quaternion.x);
            writer.Write(quaternion.y);
            writer.Write(quaternion.z);
            writer.Write(quaternion.w);
        }

        /// <summary>
        /// Write the Quaternion instance to the stream using the BinaryWriter.
        /// </summary>
        public static void Write(this BinaryWriter writer, Color color)
        {
            writer.Write(color.r);
            writer.Write(color.g);
            writer.Write(color.b);
            writer.Write(color.a);
        }

        /// <summary>
        /// Write the Bounds instance to the stream using the BinaryWriter.
        /// </summary>
        public static void Write(this BinaryWriter writer, Bounds bounds)
        {
            writer.Write(bounds.center);
            writer.Write(bounds.size);
        }

        /// <summary>
        /// Write the Matrix4x4 instance to the stream using the BinaryWriter.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static void Write(this BinaryWriter writer, Matrix4x4 matrix4x4)
        {
            writer.Write(matrix4x4.GetColumn(0));
            writer.Write(matrix4x4.GetColumn(1));
            writer.Write(matrix4x4.GetColumn(2));
            writer.Write(matrix4x4.GetColumn(3));
        }

        /// <summary>
        /// Write the Range instance to the stream using the BinaryWriter.
        /// </summary>
        public static void Write(this BinaryWriter writer, Range item)
        {
            writer.Write(item.Start.Value);
            writer.Write(item.End.Value); // We do not support from end ranges.
        }


        /// <summary>
        /// Write the IBinarySerializable instance to the stream using the BinaryWriter.
        /// </summary>
        public static void Write(this BinaryWriter writer, IBinarySerializable item)
        {
            item.Write(writer);
        }

        /// <summary>
        /// Write the object instance to the stream using the BinaryWriter.
        /// </summary>
        /// <remarks>Consider using the alternative generic methods instead as writing untyped data is pretty slow (and also might take up mare data).</remarks>
        /// <exception cref="NotSupportedException">Gets thrown when the method is not able to write instances of the specified type</exception>
        public static void Write<T>(this BinaryWriter writer, object item)
        {
            if (item is IBinarySerializable serializable)
            {
                serializable.Write(writer);
                return;
            }

            switch (typeof(T))
            {
                case { } t when t == typeof(string):
                    writer.Write((string)item);
                    break;
                case { } t when t == typeof(int):
                    writer.Write((int)item);
                    break;
                case { } t when t == typeof(uint):
                    writer.Write((uint)item);
                    break;
                case { } t when t == typeof(short):
                    writer.Write((short)item);
                    break;
                case { } t when t == typeof(ushort):
                    writer.Write((ushort)item);
                    break;
                case { } t when t == typeof(long):
                    writer.Write((long)item);
                    break;
                case { } t when t == typeof(ulong):
                    writer.Write((ulong)item);
                    break;
                case { } t when t == typeof(char):
                    writer.Write((char)item);
                    break;
                case { } t when t == typeof(byte):
                    writer.Write((byte)item);
                    break;
                case { } t when t == typeof(sbyte):
                    writer.Write((sbyte)item);
                    break;
                case { } t when t == typeof(float):
                    writer.Write((float)item);
                    break;
                case { } t when t == typeof(double):
                    writer.Write((double)item);
                    break;
                case { } t when t == typeof(Guid):
                    writer.Write((Guid)item);
                    break;
                case { } t when t == typeof(bool):
                    writer.Write((bool)item);
                    break;
                case { } t when t == typeof(decimal):
                    writer.Write((decimal)item);
                    break;
                case { } t when t == typeof(Range):
                    writer.Write((Range)item);
                    break;
                case { } t when t == typeof(DateTime):
                    writer.Write((DateTime)item);
                    break;
                case { } t when t == typeof(DateTimeOffset):
                    writer.Write((DateTimeOffset)item);
                    break;
                case { } t when t == typeof(TimeSpan):
                    writer.Write((TimeSpan)item);
                    break;
                case { } t when t == typeof(Version):
                    writer.Write((Version)item);
                    break;

                case { } t when t == typeof(Vector2):
                    writer.Write((Vector2)item);
                    break;
                case { } t when t == typeof(Vector3):
                    writer.Write((Vector3)item);
                    break;
                case { } t when t == typeof(Vector4):
                    writer.Write((Vector4)item);
                    break;
                case { } t when t == typeof(Quaternion):
                    writer.Write((Quaternion)item);
                    break;
                case { } t when t == typeof(Rect):
                    writer.Write((Rect)item);
                    break;
                case { } t when t == typeof(Color):
                    writer.Write((Color)item);
                    break;

                case { } t when t == typeof(Bounds):
                    writer.Write((Bounds)item);
                    break;
                case { } t when t == typeof(Matrix4x4):
                    writer.Write((Matrix4x4)item);
                    break;

                default: throw new NotSupportedException($"TypeId is not supported: {typeof(T)}");
            }
        }

        /// <summary>
        /// Write the Guid instance to the stream using the BinaryWriter.
        /// </summary>
        public static void Write(this BinaryWriter writer, Guid item)
        {
            writer.Write(item.ToByteArray());
        }

        /// <summary>
        /// Write the DateTime instance to the stream using the BinaryWriter.
        /// </summary>
        public static void Write(this BinaryWriter writer, DateTime item)
        {
            writer.Write((byte)item.Kind);
            writer.Write(item.Ticks);
        }
        
        /// <summary>
        /// Write the DateTime instance to the stream using the BinaryWriter.
        /// </summary>
        public static void Write(this BinaryWriter writer, DateTimeOffset item)
        {
            writer.Write(item.Ticks);
            writer.Write(item.Offset);
        }

        /// <summary>
        /// Write the TimeSpan instance to the stream using the BinaryWriter.
        /// </summary>
        public static void Write(this BinaryWriter writer, TimeSpan item)
        {
            writer.Write(item.Ticks);
        }

        /// <summary>
        /// Write the Version instance to the stream using the BinaryWriter.
        /// </summary>
        public static void Write(this BinaryWriter writer, Version item)
        {
            writer.Write(item.Major);
            writer.Write(item.Minor);
            writer.Write(item.Build);
            writer.Write(item.Revision);
        }

        /// <summary>
        /// Delegate writing to the stream to the assigned write action.
        /// </summary>
        public static void Write<TSerializable>(this BinaryWriter writer, TSerializable item,
            Action<BinaryWriter, TSerializable> write)
        {
            write(writer, item);
        }

        /// <summary>
        /// Write the generic object instance optional to the stream using the BinaryWriter.
        /// </summary>
        /// <remarks>This method will prefix the value with a bool in the stream to indicate if the variable was unassigned (null) or not.</remarks>
        public static void WriteOptional<T>(this BinaryWriter writer, T item)
            where T : class
        {
            if (item != null)
            {
                writer.Write(true);
                Write<T>(writer, item);
            }
            else
            {
                writer.Write(false);
            }
        }

        /// <summary>
        /// Write the generic struct instance optional to the stream using the BinaryWriter.
        /// </summary>
        /// <remarks>This method will prefix the value with a bool in the stream to indicate if the variable was unassigned (null) or not.</remarks>
        public static void WriteOptional<T>(this BinaryWriter writer, T? item)
            where T : struct
        {
            if (item.HasValue)
            {
                writer.Write(true);
                Write<T>(writer, item.Value);
            }
            else
            {
                writer.Write(false);
            }
        }

        /// <summary>
        /// Write the object instance to the stream using the BinaryWriter.
        /// </summary>
        /// <remarks>Consider using the alternative generic methods instead as writing untyped data is pretty slow (and also will take up mare data).</remarks>
        /// <exception cref="NotSupportedException">Gets thrown when the method is not able to write instances of the specified type</exception>
        public static void WriteTyped(this BinaryWriter writer, object item)
        {
            var typeId = TypeIdConverter.ToTypeId(item);
            writer.Write((byte)typeId);

            switch (typeId)
            {
                case TypeId.String:
                    writer.Write((string)item);
                    break;
                case TypeId.Char:
                    writer.Write((char)item);
                    break;
                case TypeId.Boolean:
                    writer.Write((bool)item);
                    break;
                case TypeId.SByte:
                    writer.Write((sbyte)item);
                    break;
                case TypeId.Byte:
                    writer.Write((byte)item);
                    break;
                case TypeId.Int16:
                    writer.Write((short)item);
                    break;
                case TypeId.Int32:
                    writer.Write((int)item);
                    break;
                case TypeId.Int64:
                    writer.Write((long)item);
                    break;
                case TypeId.UInt16:
                    writer.Write((ushort)item);
                    break;
                case TypeId.UInt32:
                    writer.Write((uint)item);
                    break;
                case TypeId.UInt64:
                    writer.Write((ulong)item);
                    break;
                case TypeId.Single:
                    writer.Write((float)item);
                    break;
                case TypeId.Double:
                    writer.Write((double)item);
                    break;
                case TypeId.Decimal:
                    writer.Write((decimal)item);
                    break;
                case TypeId.DateTime:
                    writer.Write((DateTime)item);
                    break;
                case TypeId.DateTimeOffset:
                    writer.Write((DateTimeOffset)item);
                    break;
                case TypeId.TimeSpan:
                    writer.Write((TimeSpan)item);
                    break;
                case TypeId.Guid:
                    writer.Write((Guid)item);
                    break;
                case TypeId.Version:
                    writer.Write((Version)item);
                    break;
                case TypeId.Range:
                    writer.Write((Range)item);
                    break;

                case TypeId.Vector2:
                    writer.Write((Vector2)item);
                    break;
                case TypeId.Vector3:
                    writer.Write((Vector3)item);
                    break;
                case TypeId.Vector4:
                    writer.Write((Vector4)item);
                    break;
                case TypeId.Quaternion:
                    writer.Write((Quaternion)item);
                    break;
                case TypeId.Rect:
                    writer.Write((Rect)item);
                    break;
                case TypeId.Color:
                    writer.Write((Color)item);
                    break;

                case TypeId.Bounds:
                    writer.Write((Bounds)item);
                    break;
                case TypeId.Matrix4x4:
                    writer.Write((Matrix4x4)item);
                    break;
                
                case TypeId.None: break;
                default: throw new NotSupportedException($"TypeId is not supported: {typeId}");
            }
        }

        /// <summary>
        /// Write a set of values to the stream using the BinaryWriter.
        /// This method will prefix the list with an integer indicating the number of items.
        /// </summary>
        public static void WriteMany<T>(this BinaryWriter writer, IReadOnlyList<T> items)
        {
            writer.Write(items.Count);

            foreach (var item in items)
            {
                Write<T>(writer, item);
            }
        }
        
        /// <summary>
        /// Write a set of values to the stream using the BinaryWriter.
        /// This method will prefix the list with an integer indicating the number of items.
        /// </summary>
        public static void WriteMany(this BinaryWriter writer, IReadOnlyList<IBinarySerializable> items)
        {
            writer.Write(items.Count);

            foreach (var item in items)
            {
                Write(writer, item);
            }
        }
    }
}