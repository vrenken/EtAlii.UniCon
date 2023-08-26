namespace EtAlii.UniCon.Editor
{
    using System;
    using Serilog.Events;

    // ReSharper disable once InconsistentNaming
    public static class LogEventLine
    {
        public static string GetMessage(LogEvent logEvent)
        {
            var color = logEvent.Level switch
            {
                LogEventLevel.Verbose => $"<color={WellKnownColor.LogIconVerboseHexColor}>",
                LogEventLevel.Information => $"<color={WellKnownColor.LogIconInformationHexColor}>",
                LogEventLevel.Debug => $"<color={WellKnownColor.LogIconDebugHexColor}>",
                LogEventLevel.Warning => $"<color={WellKnownColor.LogIconWarningHexColor}>",
                LogEventLevel.Error => $"<color={WellKnownColor.LogIconErrorHexColor}>",
                LogEventLevel.Fatal => $"<color={WellKnownColor.LogIconFatalHexColor}>",
                _ => throw new ArgumentOutOfRangeException(nameof(logEvent.Level))
            };
            return $"{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss.fffffff} {color}\u25CF</color> " + MarkerMessageTemplateRenderer.Render(logEvent.MessageTemplate, logEvent.Properties);
        }
    }    
}
