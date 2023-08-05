namespace EtAlii.UniCon
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Serilog.Events;
    using UniRx;
    
    public class LogSink
    {
        public TimeSpan Window = TimeSpan.FromHours(1);
        public static readonly LogSink Instance = new();

        private readonly LogEventWriteStream _writeStream = new ();
        public int EventCount { get; private set; }

        private readonly TimeSpan _interval = TimeSpan.FromMilliseconds(100);
        private CancellationTokenSource _cancellationTokenSource = new();
        private Task _task;

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

        public IObservable<LogEvent> Observe()
        {
            var subject = new ReplaySubject<LogEvent>(Window);

            if (_task != null)
            {
                _cancellationTokenSource.Cancel();
                _task.Wait();
                _task = null;
                _cancellationTokenSource = new CancellationTokenSource();
            }

            _task = Task.Run(() =>
            {
                using var readStream = new LogEventReadStream();

                do
                {
                    while (readStream.HasMoreData && !_cancellationTokenSource.IsCancellationRequested)
                    {
                        var logEvent = readStream.ReadNext();
                        subject.OnNext(logEvent);
                    }

                    Task.Delay(_interval).Wait();
                } while (!_cancellationTokenSource.IsCancellationRequested);

            }, _cancellationTokenSource.Token);
            return subject;
        }
    }
}