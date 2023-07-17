namespace EtAlii.UniCon
{
    using System;
    using System.Collections.Generic;
    using Serilog.Events;
    using UniRx;

    public class LogSink
    {
        public TimeSpan Window = TimeSpan.FromHours(1);
        public static readonly LogSink Instance = new();
        private readonly Queue<LogEvent> _logEvents = new();
        private readonly object _lockObject = new();
        private readonly Subject<LogEvent> _subject = new();

        private LogSink() { }

        public void Add(LogEvent logEvent)
        {
            lock (_lockObject)
            {
                _logEvents.Enqueue(logEvent);
                _subject.OnNext(logEvent);
                var completed = false;
                do
                {
                    var peek = _logEvents.Peek();
                    if (peek.Timestamp < DateTimeOffset.Now - Window)
                    {
                        _logEvents.Dequeue();
                    }
                    else
                    {
                        completed = true;
                    }
                } while (!completed);
            }
        }

        public IObservable<LogEvent> Observe()
        {
            lock (_lockObject)
            {
                return _logEvents.ToObservable().Concat(_subject);
            }
        }
    }
}