// Copyright (c) Peter Vrenken. All rights reserved. 

namespace EtAlii.Unicon
{
    using System.IO;
    using Serilog.Events;

    // ReSharper disable once InconsistentNaming
    public static partial class LogEntrySerialization
    {
        public static void Serialize(LogEntry logEntry, BinaryWriter writer)
        {
            writer.Write(logEntry.Index);
            writer.Write(logEntry.LogEvent.Timestamp);
            writer.Write((byte)logEntry.LogEvent.Level);
            SerializeException(logEntry.LogEvent.Exception, writer);
            SerializeMessageTemplate(logEntry.LogEvent.MessageTemplate, writer);
            SerializeProperties(logEntry.LogEvent.Properties, writer);
        }

        public static LogEntry Deserialize(BinaryReader reader)
        {
            var index = reader.ReadInt64();
            var timestamp = reader.ReadDateTimeOffset();
            var level = (LogEventLevel)reader.Read<byte>();
            var exception = DeserializeException(reader);
            var messageTemplate = DeserializeMessageTemplate(reader);
            var properties = DeserializeProperties(reader);

            var logEvent = new LogEvent(timestamp, level, exception, messageTemplate, properties);
            return new LogEntry
            {
                Index = index,
                LogEvent = logEvent
            };
        }
    }
}