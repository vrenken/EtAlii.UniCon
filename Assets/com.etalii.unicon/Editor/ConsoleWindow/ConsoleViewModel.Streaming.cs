namespace EtAlii.UniCon.Editor
{
    using System;
    using Serilog.Events;
    using UniRx;

    public partial class ConsoleViewModel
    {
        public IObservable<LogEventViewModel> Stream;
        public event Action StreamChanged;

        private void ConfigureStream()
        {
            var stream = LogSink.Instance
                .Observe();

            stream = stream.Where(logLevel =>
            {
                return logLevel.Level switch
                {
                    LogEventLevel.Verbose => Settings.LogLevel.HasFlag(LogLevel.Verbose),
                    LogEventLevel.Information => Settings.LogLevel.HasFlag(LogLevel.Information),
                    LogEventLevel.Debug => Settings.LogLevel.HasFlag(LogLevel.Debug),
                    LogEventLevel.Warning => Settings.LogLevel.HasFlag(LogLevel.Warning),
                    LogEventLevel.Error => Settings.LogLevel.HasFlag(LogLevel.Error),
                    LogEventLevel.Fatal => Settings.LogLevel.HasFlag(LogLevel.Fatal),
                    _ => throw new ArgumentOutOfRangeException(nameof(logLevel.Level))
                };
            });
            
            
            
            Stream = stream.Select(logEvent => new LogEventViewModel(logEvent));
            StreamChanged?.Invoke();            
        }
    }    
}
