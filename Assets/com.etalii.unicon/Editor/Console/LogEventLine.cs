namespace EtAlii.UniCon.Editor
{
    using System;
    using Serilog.Events;

    public static class LogEventLine
    {
        public static string GetMessage(LogEvent logEvent)
        {
            var color = logEvent.Level switch
            {
                LogEventLevel.Verbose => "<color=#5A5A5A>",
                LogEventLevel.Information => "<color=white>",
                LogEventLevel.Debug => "<color=#808080>",
                LogEventLevel.Warning => "<color=yellow>",
                LogEventLevel.Error => "<color=red>",
                LogEventLevel.Fatal => "<color=red>",
                _ => throw new ArgumentOutOfRangeException(nameof(logEvent.Level))
            };
            return $"{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss.fffffff} {color}\u25CF</color> " + MarkerMessageTemplateRenderer.Render(logEvent.MessageTemplate, logEvent.Properties);
        }
    }    
}
