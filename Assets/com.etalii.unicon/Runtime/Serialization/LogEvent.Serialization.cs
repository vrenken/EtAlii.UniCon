// Copyright (c) Peter Vrenken. All rights reserved. 

namespace EtAlii.Unicon
{
    using System.IO;
    using Serilog.Events;

    public static partial class LogEventSerialization
    {
        public static void Serialize(LogEvent logEvent, BinaryWriter writer)
        {
            writer.Write(logEvent.Timestamp);
            writer.Write((byte)logEvent.Level);
            SerializeException(logEvent.Exception, writer);
            SerializeMessageTemplate(logEvent.MessageTemplate, writer);
            SerializeProperties(logEvent.Properties, writer);
        }

        public static LogEvent Deserialize(BinaryReader reader)
        {
            var timestamp = reader.ReadDateTimeOffset();
            var level = (LogEventLevel)reader.Read<byte>();
            var exception = DeserializeException(reader);
            var messageTemplate = DeserializeMessageTemplate(reader);
            var properties = DeserializeProperties(reader);

            return new LogEvent(timestamp, level, exception, messageTemplate, properties);
        }
    }
}