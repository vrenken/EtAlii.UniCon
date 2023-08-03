namespace EtAlii.UniCon
{
    using System;
    using Serilog.Events;
    using UniRx;

    public class LogSink
    {
        public TimeSpan Window = TimeSpan.FromHours(1);
        public static readonly LogSink Instance = new();

        private ReplaySubject<LogEvent> _subject;

        public int EventCount { get; private set; }

        private LogSink()
        {
            Clear();
        }

        public void Clear()
        {
            _subject = new ReplaySubject<LogEvent>(Window);
            EventCount = 0;
        }
        
        public void Add(LogEvent logEvent)
        {
            EventCount += 1;
            _subject.OnNext(logEvent);
        }

        public IObservable<LogEvent> Observe()
        {
            return _subject;
        }
    }
}