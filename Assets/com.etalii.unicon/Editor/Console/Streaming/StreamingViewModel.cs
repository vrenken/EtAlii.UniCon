namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Linq;
    using Serilog.Events;
    using UniRx;
    using UnityEngine.UIElements;

    public class StreamingViewModel
    {
        public IObservable<LogEvent> Stream;
        
        public readonly ReactiveCommand<ClickEvent> ToggleScrollToTail = new();

        /// <summary>
        /// Gets raised when the stream has changed and the view needs to update itself accordingly.
        /// </summary>
        public event Action StreamChanged;

        private FiltersViewModel _filtersViewModel;
        private ExpressionViewModel _expressionViewModel;

        public void Bind(FiltersViewModel filtersViewModel, ExpressionViewModel expressionViewModel)
        {
            _filtersViewModel = filtersViewModel;
            _expressionViewModel = expressionViewModel;
            ToggleScrollToTail.Subscribe(_ =>
            {
                UserSettings.instance.ScrollToTail.Value = !UserSettings.instance.ScrollToTail.Value;
            });
        }
        
        /// <summary>
        /// Reconfigure the stream with the right filtering rules applied.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void ConfigureStream()
        {
            var stream = LogSink.Instance
                .Observe();

            stream = stream
                .Where(logEvent =>
                {
                    if (UserSettings.instance.UseSerilogSource.Value)
                    {
                        // Only the availability of the property is already sufficient.
                        if(!logEvent.Properties.TryGetValue(WellKnownProperties.IsUnityLogEvent, out _))
                        {
                            return true;
                        }
                    }
                    if (UserSettings.instance.UseUnitySource.Value)
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
                    if (UserSettings.instance.LogLevel.Value == LogLevel.None && !UserSettings.instance.ShowExceptions.Value)
                    {
                        return true;
                    }

                    if (UserSettings.instance.ShowExceptions.Value && logEvent.Exception != null)
                    {
                        return true;
                    }

                    return logEvent.Level switch
                    {
                        LogEventLevel.Verbose => UserSettings.instance.LogLevel.Value.HasFlag(LogLevel.Verbose),
                        LogEventLevel.Information => UserSettings.instance.LogLevel.Value.HasFlag(LogLevel.Information),
                        LogEventLevel.Debug => UserSettings.instance.LogLevel.Value.HasFlag(LogLevel.Debug),
                        LogEventLevel.Warning => UserSettings.instance.LogLevel.Value.HasFlag(LogLevel.Warning),
                        LogEventLevel.Error => UserSettings.instance.LogLevel.Value.HasFlag(LogLevel.Error),
                        LogEventLevel.Fatal => UserSettings.instance.LogLevel.Value.HasFlag(LogLevel.Fatal),
                        _ => throw new ArgumentOutOfRangeException(nameof(logEvent.Level))
                    };
                }).Where(logEvent =>
                {
                    var filters = _filtersViewModel.CustomFilters
                        .Where(f => f.IsActive.Value && !f.IsEditing.Value)
                        .ToArray();
                    if(!filters.Any()) return true;
                    var isMatchedByFilter = filters
                        .Select(f => LogFilterIsValid(f, logEvent))
                        .Any(r => r);
                    return isMatchedByFilter;
                }).Where(logEvent =>
                {
                    if (_expressionViewModel.ExpressionFilter.CompiledExpression.Value != null && 
                        _expressionViewModel.ExpressionFilter.IsActive.Value)
                    {
                        return LogFilterIsValid(_expressionViewModel.ExpressionFilter, logEvent);
                    }

                    return true;
                });
            
            Stream = stream;
            StreamChanged?.Invoke();            
        }

        private bool LogFilterIsValid(LogFilter rule, LogEvent logEvent)
        {
            var result = rule.CompiledExpression.Value?.Invoke(logEvent);
            if (result is not ScalarValue scalarValue)
            {
                return false;
            }

            return scalarValue.Value is not false;
        }
    }
}
