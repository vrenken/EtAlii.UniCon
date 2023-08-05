// Copyright (c) Peter Vrenken. All rights reserved. 

namespace EtAlii.Unicon
{
    using System.IO;

    /// <summary>
    /// Interface to indicate that a class can be used in a binary serialized composition.
    /// </summary>
    public interface IBinarySerializable
    {
        /// <summary>
        /// Write the instance state to the BinaryWriter. 
        /// </summary>
        /// <param name="writer">The writer to write the state to.</param>
        void Write(BinaryWriter writer);

        /// <summary>
        /// Read the state of the instance from the BinaryReader.
        /// </summary>
        /// <param name="reader">The reader to read the state from.</param>
        void Read(BinaryReader reader);
    }
}