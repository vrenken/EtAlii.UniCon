namespace EtAlii.UniCon
{
    using System;
    using System.IO;
    using System.Threading;
    using EtAlii.Unicon;
    using Serilog.Events;

    // ReSharper disable once InconsistentNaming
    public sealed class LogEventWriteStream : IDisposable
    {
        // ReSharper disable once InconsistentNaming
        public const string LogFileNameWithoutExtension = "Logs/unicon-log";
        
        private readonly FileStream _dataWriteStream;
        private readonly BinaryWriter _dataWriter;

        private readonly FileStream _indexWriteStream;
        private readonly BinaryWriter _indexWriter;

        public static readonly object LockObject = new object();

        private long _index;
        
        public LogEventWriteStream()
        {
            _indexWriteStream = new FileStream($"{LogFileNameWithoutExtension}.index", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            _indexWriteStream.Seek(0, SeekOrigin.End);
            _indexWriter = new BinaryWriter(_indexWriteStream);

            _dataWriteStream = new FileStream($"{LogFileNameWithoutExtension}.bin", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            _dataWriteStream.Seek(0, SeekOrigin.End);
            _dataWriter = new BinaryWriter(_dataWriteStream);
        }
        
        public void Write(LogEvent logEvent)
        {
            Monitor.Enter(LockObject);
            try
            {
                var entry = new LogEntry { Index = _index++, LogEvent = logEvent };
                LogEntrySerialization.Serialize(entry, _dataWriter);
                _dataWriter.Flush();
                _dataWriteStream.Flush();
                _indexWriter.Write(_dataWriteStream.Position);
            }
            finally
            {
                Monitor.Exit(LockObject);
            }
        }

        public void Dispose()
        {
            _indexWriter.Dispose();
            _indexWriteStream.Dispose();

            _dataWriter.Dispose();
            _dataWriteStream.Dispose();
        }
    }
}