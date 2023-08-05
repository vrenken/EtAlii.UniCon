// Copyright (c) Peter Vrenken. All rights reserved. 

namespace EtAlii.Unicon
{
    using System.Collections.Generic;
    using System.IO;
    using Serilog.Events;

    public static partial class LogEntrySerialization
    {
        
        private static void SerializeProperties(IReadOnlyDictionary<string, LogEventPropertyValue> logEventProperties, BinaryWriter writer)
        {
            writer.Write(logEventProperties.Count);
            foreach (var kvp in logEventProperties)
            {
                writer.Write(kvp.Key);
                SerializeLogEventPropertyValue(kvp.Value, writer);
            }
        }

        private static IEnumerable<LogEventProperty> DeserializeProperties(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var name = reader.ReadString();
                var value = DeserializePropertyValue(reader);
                yield return new LogEventProperty(name, value);
            }
        }

        private static void SerializeLogEventPropertyValue(LogEventPropertyValue value, BinaryWriter writer)
        {
            switch (value)
            {
                case DictionaryValue:
                    writer.Write((byte)PropertyValueType.ScalarValue);
                    writer.WriteTyped("TODO");
                    break;
                case ScalarValue scalarValue:
                    writer.Write((byte)PropertyValueType.ScalarValue);
                    writer.WriteTyped(scalarValue.Value);
                    break;
                case SequenceValue:
                    writer.Write((byte)PropertyValueType.ScalarValue);
                    writer.WriteTyped("TODO");
                    break;
                case StructureValue:
                    writer.Write((byte)PropertyValueType.ScalarValue);
                    writer.WriteTyped("TODO");
                    break;
            }
        }

        private static LogEventPropertyValue DeserializePropertyValue(BinaryReader reader)
        {
            LogEventPropertyValue propertyValue = null; 
            var propertyValueType = (PropertyValueType)reader.ReadByte();
            switch (propertyValueType) 
            {
                case PropertyValueType.DictionaryValue:
                    propertyValue = new ScalarValue("TODO");
                    break;
                case PropertyValueType.ScalarValue:
                    var value = reader.ReadTyped();
                    propertyValue = new ScalarValue(value);
                    break;
                case PropertyValueType.SequenceValue:
                    propertyValue = new ScalarValue("TODO");
                    break;
                case PropertyValueType.StructureValue:
                    propertyValue = new ScalarValue("TODO");
                    break;
            }
            return propertyValue;
        }
    }
}