namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Linq;
    using Serilog.Events;

    public partial class StreamingViewModel
    {
        private PipelineItem FilterBySource(PipelineItem item)
        {
            var logEvent = item.LogEvent;
            if (!logEvent.Properties.TryGetValue(WellKnownProperty.LogSource, out var propertyValue))
            {
                item.LogEvent = null;
                return item;
            }

            if (propertyValue is not ScalarValue { Value: string logSource })
            {
                item.LogEvent = null;
                return item;
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
                item.LogEvent = null;
            }

            return item;
        }

        private PipelineItem FilterByLogLevel(PipelineItem item)
        {
            if (item.LogEvent == null) return item;

            var logEvent = item.LogEvent;

            if (UserSettings.instance.LogLevel.Value == LogLevel.None &&
                !UserSettings.instance.ShowExceptions.Value)
            {
                return item;
            }

            if (UserSettings.instance.ShowExceptions.Value && logEvent.Exception != null)
            {
                return item;
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
                item.LogEvent = null;
            }

            return item;
        }

        private PipelineItem FilterByCustomFilter(PipelineItem item)
        {
            if (item.LogEvent == null) return item;

            var logEvent = item.LogEvent;

            var filters = _filtersViewModel.CustomFilters
                .Where(f => f.IsActive.Value && !f.IsEditing.Value)
                .ToArray();
            if (!filters.Any()) return item;
            var isMatchedByFilter = filters
                .Select(f => LogFilterIsValid(f, logEvent))
                .Any(r => r);
            if (!isMatchedByFilter)
            {
                item.LogEvent = null;
            }

            return item;
        }

        private PipelineItem FilterByExpression(PipelineItem item)
        {
            if (item.LogEvent == null) return item;

            var logEvent = item.LogEvent;

            if (_expressionViewModel.ExpressionFilter.CompiledExpression.Value != null && 
                _expressionViewModel.ExpressionFilter.IsActive.Value)
            {
                var isValid = LogFilterIsValid(_expressionViewModel.ExpressionFilter, logEvent);
                if (!isValid)
                {
                    item.LogEvent = null;
                }
            }

            return item;
        }

        private void OutputLogEvent(PipelineItem item)
        {
            if (item.LogEvent != null)
            {
                _subject.OnNext(item.LogEvent);
            }
        }
    }
}
