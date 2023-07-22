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

        /// <summary>
        /// Reconfigure the stream with the right filtering rules applied.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void ConfigureStream()
        {
            var stream = LogSink.Instance
                .Observe();

            stream = stream
                .Where(logEvent =>
                {
                    if (Settings.UseSerilogSource.Value)
                    {
                        // Only the availability of the property is already sufficient.
                        if(!logEvent.Properties.TryGetValue(WellKnownProperties.IsUnityLogEvent, out _))
                        {
                            return true;
                        }
                    }
                    if (Settings.UseUnitySource.Value)
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
                    if (Settings.ShowExceptions.Value && logEvent.Exception != null)
                    {
                        return true;
                    }
                    return logEvent.Level switch
                    {
                        LogEventLevel.Verbose => Settings.LogLevel.Value.HasFlag(LogLevel.Verbose),
                        LogEventLevel.Information => Settings.LogLevel.Value.HasFlag(LogLevel.Information),
                        LogEventLevel.Debug => Settings.LogLevel.Value.HasFlag(LogLevel.Debug),
                        LogEventLevel.Warning => Settings.LogLevel.Value.HasFlag(LogLevel.Warning),
                        LogEventLevel.Error => Settings.LogLevel.Value.HasFlag(LogLevel.Error),
                        LogEventLevel.Fatal => Settings.LogLevel.Value.HasFlag(LogLevel.Fatal),
                        _ => throw new ArgumentOutOfRangeException(nameof(logEvent.Level))
                    };
                }).Where(logEvent =>
                {
                    foreach (var rule in SelectedFilterRules)
                    {
                        if (!RuleIsValid(rule, logEvent))
                        {
                            return false;
                        }
                    }

                    if (ActiveFilterRule.CompiledExpression != null)
                    {
                        if (!RuleIsValid(ActiveFilterRule, logEvent))
                        {
                            return false;
                        }
                    }
                    
                    return true;
                });
            
            Stream = stream;
            StreamChanged?.Invoke();            
        }

        private bool RuleIsValid(FilterRule rule, LogEvent logEvent)
        {
            var result = rule.CompiledExpression?.Invoke(logEvent);
            if (result is not ScalarValue scalarValue)
            {
                return false;
            }

            return scalarValue.Value is not false;
        }
    }    
}
