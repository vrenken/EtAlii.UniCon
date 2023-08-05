namespace EtAlii.UniCon
{
    using System;
    using System.IO;
    using System.Threading;
    using EtAlii.Unicon;
    using Serilog.Events;

    public sealed class LogEventReadStream : IDisposable
    {
        private readonly FileStream _dataReadStream;
        private readonly BinaryReader _dataReader;

        private readonly FileStream _indexReadStream;
        private readonly BinaryReader _indexReader;

        public bool HasMoreData => _dataReadStream.Position < _dataReadStream.Length;// Interlocked.Read(ref LogEventWriteStream.LogEventCounter);

        public LogEventReadStream()
        {
            _indexReadStream = new FileStream($"{LogEventWriteStream.LogFileNameWithoutExtension}.index", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _indexReadStream.Seek(0, SeekOrigin.Begin);
            _indexReader = new BinaryReader(_indexReadStream);

            _dataReadStream = new FileStream($"{LogEventWriteStream.LogFileNameWithoutExtension}.bin", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _dataReadStream.Seek(0, SeekOrigin.Begin);
            _dataReader = new BinaryReader(_dataReadStream);
        }
        
        public LogEvent ReadNext()
        {
            Monitor.Enter(LogEventWriteStream.LockObject);
            try
            {
                var logEvent = LogEventSerialization.Deserialize(_dataReader);
                return logEvent;
            }
            finally
            {
                Monitor.Exit(LogEventWriteStream.LockObject);
            }
        }

        public LogEvent ReadPrevious()
        {
            Monitor.Enter(LogEventWriteStream.LockObject);
            try
            {
                _indexReadStream.Seek(sizeof(long), SeekOrigin.Current);
                var readPosition = _indexReader.ReadInt64();
                _dataReadStream.Seek(readPosition, SeekOrigin.Begin);
                var logEvent = LogEventSerialization.Deserialize(_dataReader);
                _dataReadStream.Seek(readPosition, SeekOrigin.Begin);
                return logEvent;
            }
            finally
            {
                Monitor.Exit(LogEventWriteStream.LockObject);
            }
        }

        public void Dispose()
        {
            _indexReader.Dispose();
            _indexReadStream.Dispose();

            _dataReader.Dispose();
            _dataReadStream.Dispose();
        }
    }
}