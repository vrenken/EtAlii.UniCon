namespace EtAlii.UniCon.Editor
{
    public class ConsoleSettings
    {
        public LogLevel LogLevel = LogLevel.Information | LogLevel.Debug | LogLevel.Warning | LogLevel.Error | LogLevel.Fatal;

        /// <summary>
        /// Set this value to true to show any log event that contains information about an exception. This overrules the default log levels.
        /// </summary>
        public bool ShowExceptions = true;
        
        /// <summary>
        /// Set this value to true to show all log events that are gathered by Serilog. 
        /// </summary>
        public bool UseSerilogSource = true;

        /// <summary>
        /// Set this value to true to show all log events that are gathered by the default Unity debug logs. 
        /// </summary>
        public bool UseUnitySource = false;

        /// <summary>
        /// Set this property to true to keep scrolling to any new log events received.
        /// </summary>
        public bool ScrollToTail = true;

        public bool ShowFilter = true;
        public bool ShowRules = false;
    }    
}
