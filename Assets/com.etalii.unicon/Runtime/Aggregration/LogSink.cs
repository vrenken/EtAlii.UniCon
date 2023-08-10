namespace EtAlii.UniCon
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EtAlii.Unicon;
    using Serilog.Events;
    using UniRx;
    
    public class LogSink
    {
        public TimeSpan Window = TimeSpan.FromHours(1);
        public static readonly LogSink Instance = new();

        private readonly LogEventWriteStream _writeStream = new ();
        public int EventCount { get; private set; }

        private readonly TimeSpan _interval = TimeSpan.FromMilliseconds(100);

        private CancellationTokenSource _forwardCancellationTokenSource = new();
        private Task _forwardTask;

        private CancellationTokenSource _backwardCancellationTokenSource = new();
        private Task _backwardTask;

        private LogSink()
        {
            Clear();
        }

        public void Clear()
        {
            EventCount = 0;
        }
        
        public void Add(LogEvent logEvent)
        {
            _writeStream.Write(logEvent);
            
            EventCount += 1;
        }

        public IObservable<LogEntry> ObserveForward(long position)
        {
            var subject = new ReplaySubject<LogEntry>(Window);

            if (_forwardTask != null)
            {
                _forwardCancellationTokenSource.Cancel();
                _forwardTask.Wait();
                _forwardTask = null;
                _forwardCancellationTokenSource = new CancellationTokenSource();
            }

            _forwardTask = Task.Run(() =>
            {
                using var readStream = new LogEventReadStream(position);

                do
                {
                    while (readStream.HasMoreDataAhead && !_forwardCancellationTokenSource.IsCancellationRequested)
                    {
                        var entry = readStream.ReadNext();
                        subject.OnNext(entry);
                    }

                    Task.Delay(_interval).Wait();
                } while (!_forwardCancellationTokenSource.IsCancellationRequested);

            }, _forwardCancellationTokenSource.Token);
            return subject;
        }
        
        
        public IObservable<LogEntry> ObserveBackward(long position)
        {
            var subject = new ReplaySubject<LogEntry>(Window);

            if (_backwardTask != null)
            {
                _backwardCancellationTokenSource.Cancel();
                _backwardTask.Wait();
                _backwardTask = null;
                _backwardCancellationTokenSource = new CancellationTokenSource();
            }

            _backwardTask = Task.Run(() =>
            {
                using var readStream = new LogEventReadStream(position);

                while (readStream.HasMoreDataBehind && !_backwardCancellationTokenSource.IsCancellationRequested)
                {
                    var entry = readStream.ReadPrevious();
                    subject.OnNext(entry);

                    Task.Delay(_interval / 10).Wait();
                }

            }, _backwardCancellationTokenSource.Token);
            return subject;
        }
    }
}