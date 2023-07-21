namespace EtAlii.UniCon.Editor
{
    using UniRx;

    public class ConsoleSettings
    {
        private const LogLevel DefaultLogLevel = EtAlii.UniCon.Editor.LogLevel.Information | EtAlii.UniCon.Editor.LogLevel.Debug | EtAlii.UniCon.Editor.LogLevel.Warning | EtAlii.UniCon.Editor.LogLevel.Error | EtAlii.UniCon.Editor.LogLevel.Fatal;
        public readonly ReactiveProperty<LogLevel> LogLevel = new(DefaultLogLevel);

        /// <summary>
        /// Set this value to true to show any log event that contains information about an exception. This overrules the default log levels.
        /// </summary>
        public readonly ReactiveProperty<bool> ShowExceptions = new(true);
        
        /// <summary>
        /// Set this value to true to show all log events that are gathered by Serilog. 
        /// </summary>
        public readonly ReactiveProperty<bool> UseSerilogSource = new (true);

        /// <summary>
        /// Set this value to true to show all log events that are gathered by the default Unity debug logs. 
        /// </summary>
        public readonly ReactiveProperty<bool> UseUnitySource = new(false);

        /// <summary>
        /// Set this property to true to keep scrolling to any new log events received.
        /// </summary>
        public bool ScrollToTail = true;

        public readonly ReactiveProperty<bool> ShowFilterPanel = new(false);
        
        public float FilterPanelWidth = 150f;

        public ReactiveProperty<bool> ShowExpressionPanel = new(false);
        public float ExpressionPanelHeight = 150f;
    }    
}
