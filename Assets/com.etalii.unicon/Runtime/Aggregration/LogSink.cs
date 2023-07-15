namespace EtAlii.UniCon
{
    using Serilog.Events;
    using System.Collections.ObjectModel;

    public static class LogSink
    {
        public static readonly ObservableCollection<LogEvent> LogEvents = new();
    }
}