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

            stream = stream
                .Where(logEvent =>
                {
                    if (Settings.UseSerilogSource)
                    {
                        // Only the availability of the property is already sufficient.
                        if(!logEvent.Properties.TryGetValue(WellKnownProperties.IsUnityLogEvent, out _))
                        {
                            return true;
                        }
                    }
                    if (Settings.UseUnitySource)
                    {
                        // Only the absence of the property is already sufficient.
                        if(logEvent.Properties.TryGetValue(WellKnownProperties.IsUnityLogEvent, out _))
                        {
                            return true;
                        }
                    }
                    
                    return false;
                })
                .Where(logEvent =>
            {
                return logEvent.Level switch
                {
                    LogEventLevel.Verbose => Settings.LogLevel.HasFlag(LogLevel.Verbose),
                    LogEventLevel.Information => Settings.LogLevel.HasFlag(LogLevel.Information),
                    LogEventLevel.Debug => Settings.LogLevel.HasFlag(LogLevel.Debug),
                    LogEventLevel.Warning => Settings.LogLevel.HasFlag(LogLevel.Warning),
                    LogEventLevel.Error => Settings.LogLevel.HasFlag(LogLevel.Error),
                    LogEventLevel.Fatal => Settings.LogLevel.HasFlag(LogLevel.Fatal),
                    _ => throw new ArgumentOutOfRangeException(nameof(logEvent.Level))
                };
            });
            
            
            
            Stream = stream.Select(logEvent => new LogEventViewModel(logEvent));
            StreamChanged?.Invoke();            
        }
    }    
}
