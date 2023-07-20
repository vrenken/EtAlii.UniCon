namespace EtAlii.UniCon.Editor
{
    using System;
    using Serilog.Events;
    using UniRx;

    public partial class ConsoleViewModel
    {
        public IObservable<LogEvent> Stream;
        
        /// <summary>
        /// Gets raised when the stream has changed and the view needs to update itself accordingly.
        /// </summary>
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
                    if (Settings.ShowExceptions && logEvent.Exception != null)
                    {
                        return true;
                    }
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
                }).Where(logEvent =>
                {
                    foreach (var rule in FilterRules)
                    {
                        if(rule.CompiledExpression != null)
                        {
                            var result = rule.CompiledExpression(logEvent);
                            if (result is not ScalarValue scalarValue)
                            {
                                return false;
                            }

                            if (scalarValue.Value is false)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return true;
                });
            
            Stream = stream;
            StreamChanged?.Invoke();            
        }
    }    
}
