namespace EtAlii.UniCon.Editor
{
    using System.IO;
    using Serilog.Events;
    using Serilog.Formatting;
    using Serilog.Formatting.Display;
    using UnityEngine;

    public class LogEventLineViewModel : ScriptableObject
    {
        private LogEvent _logEvent;

        // private const string DefaultDebugOutputTemplate = "[{Level:u3}] {Message:lj}{NewLine}{Exception}";
        private const string DefaultDebugOutputTemplate = "{Message:lj}";

        private static readonly ITextFormatter Formatter = new MessageTemplateTextFormatter(DefaultDebugOutputTemplate);
        
        public string timestamp;
        public string message;
        public string level;

        public void Init(LogEvent logEvent)
        {
            _logEvent = logEvent;
            timestamp = $"{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss.fffffff}";
            level = logEvent.Level.ToString();
            using var buffer = new StringWriter();
            Formatter.Format(logEvent, buffer);
            message = buffer.ToString().Trim();
        }
    }    
}
