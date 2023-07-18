namespace EtAlii.UniCon
{
    using System;
    using System.Collections.Concurrent;
    using Serilog.Events;
    using UniRx;

    public class LogSink
    {
        public TimeSpan Window = TimeSpan.FromHours(1);
        public static readonly LogSink Instance = new();
        private readonly ConcurrentQueue<LogEvent> _logEvents = new();
        private readonly Subject<LogEvent> _subject = new();

        private LogSink() { }

        public void Add(LogEvent logEvent)
        {
            _logEvents.Enqueue(logEvent);
            _subject.OnNext(logEvent);
            do
            {
                if (!_logEvents.TryPeek(out var peek))
                {
                    break;
                }

                if (peek.Timestamp < DateTimeOffset.Now - Window)
                {
                    if (!_logEvents.TryDequeue(out _))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            } while (true);
        }

        public IObservable<LogEvent> Observe()
        {
             return _logEvents
                 .ToObservable()
                .Concat(_subject);
        }
    }
}