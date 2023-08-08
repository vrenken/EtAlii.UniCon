namespace EtAlii.UniCon
{
    using System;
    using System.IO;
    using System.Threading;
    using EtAlii.Unicon;
    
    public sealed class LogEventReadStream : IDisposable
    {
        private readonly FileStream _dataReadStream;
        private readonly BinaryReader _dataReader;

        private readonly FileStream _indexReadStream;
        private readonly BinaryReader _indexReader;

        public bool HasMoreDataAhead => _dataReadStream.Position < _dataReadStream.Length;// Interlocked.Read(ref LogEventWriteStream.LogEventCounter);
        public bool HasMoreDataBehind => _dataReadStream.Position > 0;// Interlocked.Read(ref LogEventWriteStream.LogEventCounter);

        public LogEventReadStream(long position)
        {
            _indexReadStream = new FileStream($"{LogEventWriteStream.LogFileNameWithoutExtension}.index", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _indexReadStream.Seek(0, SeekOrigin.Begin);
            _indexReader = new BinaryReader(_indexReadStream);

            _dataReadStream = new FileStream($"{LogEventWriteStream.LogFileNameWithoutExtension}.bin", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _dataReadStream.Seek(0, SeekOrigin.Begin);
            _dataReader = new BinaryReader(_dataReadStream);
        }
        
        public LogEntry ReadNext()
        {
            Monitor.Enter(LogEventWriteStream.LockObject);
            try
            {
                return LogEntrySerialization.Deserialize(_dataReader);
            }
            finally
            {
                Monitor.Exit(LogEventWriteStream.LockObject);
            }
        }

        public LogEntry ReadPrevious()
        {
            Monitor.Enter(LogEventWriteStream.LockObject);
            try
            {
                _indexReadStream.Seek(sizeof(long), SeekOrigin.Current);
                var readPosition = _indexReader.ReadInt64();
                _dataReadStream.Seek(readPosition, SeekOrigin.Begin);
                var logEntry = LogEntrySerialization.Deserialize(_dataReader);
                _dataReadStream.Seek(readPosition, SeekOrigin.Begin);
                return logEntry;
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