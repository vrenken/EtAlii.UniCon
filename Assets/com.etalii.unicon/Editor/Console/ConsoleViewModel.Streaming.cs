namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Linq;
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
                    if (UserSettings.UseSerilogSource.Value)
                    {
                        // Only the availability of the property is already sufficient.
                        if(!logEvent.Properties.TryGetValue(WellKnownProperties.IsUnityLogEvent, out _))
                        {
                            return true;
                        }
                    }
                    if (UserSettings.UseUnitySource.Value)
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
                    if (UserSettings.LogLevel.Value == LogLevel.None && !UserSettings.ShowExceptions.Value)
                    {
                        return true;
                    }

                    if (UserSettings.ShowExceptions.Value && logEvent.Exception != null)
                    {
                        return true;
                    }

                    return logEvent.Level switch
                    {
                        LogEventLevel.Verbose => UserSettings.LogLevel.Value.HasFlag(LogLevel.Verbose),
                        LogEventLevel.Information => UserSettings.LogLevel.Value.HasFlag(LogLevel.Information),
                        LogEventLevel.Debug => UserSettings.LogLevel.Value.HasFlag(LogLevel.Debug),
                        LogEventLevel.Warning => UserSettings.LogLevel.Value.HasFlag(LogLevel.Warning),
                        LogEventLevel.Error => UserSettings.LogLevel.Value.HasFlag(LogLevel.Error),
                        LogEventLevel.Fatal => UserSettings.LogLevel.Value.HasFlag(LogLevel.Fatal),
                        _ => throw new ArgumentOutOfRangeException(nameof(logEvent.Level))
                    };
                }).Where(logEvent =>
                {
                    if (CustomFilters.Any(f => f.IsActive.Value))
                    {
                        var hasValidFilter = CustomFilters
                            .Where(f => f.IsActive.Value)
                            .Select(f => CustomFilterIsValid(f, logEvent))
                            .Any(r => r);
                        if (!hasValidFilter)
                        {
                            return false;
                        }
                    }

                    if (SelectedCustomFilter.CompiledExpression != null)
                    {
                        if (!CustomFilterIsValid(SelectedCustomFilter, logEvent))
                        {
                            return false;
                        }
                    }
                    return true;
                });
            
            Stream = stream;
            StreamChanged?.Invoke();            
        }

        private bool CustomFilterIsValid(CustomFilter rule, LogEvent logEvent)
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
