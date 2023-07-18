namespace EtAlii.UniCon.Editor
{
    using System.IO;
    using Serilog.Events;
    using Serilog.Formatting;
    using Serilog.Formatting.Display;
    
    public class LogEventViewModel
    {
        public LogEvent LogEvent { get; private set; }

        // private const string DefaultDebugOutputTemplate = "[{Level:u3}] {Message:lj}{NewLine}{Exception}";
        private const string DefaultDebugOutputTemplate = "{Message:lj}";

        private static readonly ITextFormatter Formatter = new MessageTemplateTextFormatter(DefaultDebugOutputTemplate);
        
        public readonly string Timestamp;
        public readonly string Message;
        public readonly string Level;

        public LogEventViewModel(LogEvent logEvent)
        {
            LogEvent = logEvent;
            Timestamp = $"{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss.fffffff}";
            Level = logEvent.Level.ToString();
            using var buffer = new StringWriter();
            Formatter.Format(logEvent, buffer);
            Message = buffer.ToString().Trim();
        }
    }    
}
