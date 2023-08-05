namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Linq;
    using EtAlii.Unicon;
    using Serilog.Events;

    public partial class StreamingViewModel
    {
        private LogEntry FilterBySource(LogEntry entry)
        {
            var logEvent = entry.LogEvent;
            if (!logEvent.Properties.TryGetValue(WellKnownProperty.LogSource, out var propertyValue))
            {
                entry.LogEvent = null;
                return entry;
            }

            if (propertyValue is not ScalarValue { Value: string logSource })
            {
                entry.LogEvent = null;
                return entry;
            }

            var isValid = logSource switch
            {
                WellKnownPropertyValue.UnityLogSource => UserSettings.instance.UseUnitySource.Value,
                WellKnownPropertyValue.SerilogLogSource => UserSettings.instance.UseSerilogSource.Value,
                WellKnownPropertyValue.MicrosoftLogSource => UserSettings.instance.UseMicrosoftSource.Value,
                _ => false
            };
            if (!isValid)
            {
                entry.LogEvent = null;
            }

            return entry;
        }

        private LogEntry FilterByLogLevel(LogEntry entry)
        {
            if (entry.LogEvent == null) return entry;

            var logEvent = entry.LogEvent;

            if (UserSettings.instance.LogLevel.Value == LogLevel.None &&
                !UserSettings.instance.ShowExceptions.Value)
            {
                return entry;
            }

            if (UserSettings.instance.ShowExceptions.Value && logEvent.Exception != null)
            {
                return entry;
            }

            var isValid = logEvent.Level switch
            {
                LogEventLevel.Verbose => UserSettings.instance.LogLevel.Value.HasFlag(LogLevel.Verbose),
                LogEventLevel.Information => UserSettings.instance.LogLevel.Value.HasFlag(LogLevel.Information),
                LogEventLevel.Debug => UserSettings.instance.LogLevel.Value.HasFlag(LogLevel.Debug),
                LogEventLevel.Warning => UserSettings.instance.LogLevel.Value.HasFlag(LogLevel.Warning),
                LogEventLevel.Error => UserSettings.instance.LogLevel.Value.HasFlag(LogLevel.Error),
                LogEventLevel.Fatal => UserSettings.instance.LogLevel.Value.HasFlag(LogLevel.Fatal),
                _ => throw new ArgumentOutOfRangeException(nameof(logEvent.Level))
            };
            if (!isValid)
            {
                entry.LogEvent = null;
            }

            return entry;
        }

        private LogEntry FilterByCustomFilter(LogEntry entry)
        {
            if (entry.LogEvent == null) return entry;

            var logEvent = entry.LogEvent;

            var filters = _filtersViewModel.CustomFilters
                .Where(f => f.IsActive.Value && !f.IsEditing.Value)
                .ToArray();
            if (!filters.Any()) return entry;
            var isMatchedByFilter = filters
                .Select(f => LogFilterIsValid(f, logEvent))
                .Any(r => r);
            if (!isMatchedByFilter)
            {
                entry.LogEvent = null;
            }

            return entry;
        }

        private LogEntry FilterByExpression(LogEntry entry)
        {
            if (entry.LogEvent == null) return entry;

            var logEvent = entry.LogEvent;

            if (_expressionViewModel.ExpressionFilter.CompiledExpression.Value != null && 
                _expressionViewModel.ExpressionFilter.IsActive.Value)
            {
                var isValid = LogFilterIsValid(_expressionViewModel.ExpressionFilter, logEvent);
                if (!isValid)
                {
                    entry.LogEvent = null;
                }
            }

            return entry;
        }

        private void OutputLogEvent(LogEntry entry)
        {
            if (entry.LogEvent != null)
            {
                _subject.OnNext(entry);
            }
        }
    }
}
