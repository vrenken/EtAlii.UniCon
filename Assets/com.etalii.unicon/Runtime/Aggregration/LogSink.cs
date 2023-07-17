namespace EtAlii.UniCon
{
    using System;
    using Serilog.Events;
    using System.Collections.ObjectModel;

    public class LogSink
    {
        public TimeSpan Window = TimeSpan.FromHours(1);

        public static readonly LogSink Instance = new();
        
        public readonly ObservableCollection<LogEvent> LogEvents = new();
        
        private LogSink() { }

        public void Add()
        {
            
        }
    }
}