namespace EtAlii.UniCon.Editor
{
    using UniRx;
    using UnityEditor;

    [FilePath("UserSettings/UniConSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    internal class UserSettings : ScriptableSingleton<UserSettings>
    {
        private const LogLevel DefaultLogLevel = EtAlii.UniCon.Editor.LogLevel.None;
        // private const LogLevel DefaultLogLevel = EtAlii.UniCon.Editor.LogLevel.Information | EtAlii.UniCon.Editor.LogLevel.Debug | EtAlii.UniCon.Editor.LogLevel.Warning | EtAlii.UniCon.Editor.LogLevel.Error | EtAlii.UniCon.Editor.LogLevel.Fatal;
        
        /// <summary>
        /// The log level(s) that should be visualized. if no log levels are specified no filtering on log levels will happen.  
        /// </summary>
        public readonly ReactiveProperty<LogLevel> LogLevel = new(DefaultLogLevel);

        /// <summary>
        /// Set this value to true to show any log event that contains information about an exception. This overrules the default log levels.
        /// </summary>
        public readonly ReactiveProperty<bool> ShowExceptions = new(false);
        
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
        public readonly ReactiveProperty<bool> ScrollToTail = new(true);

        public readonly ReactiveProperty<bool> ShowFilterPanel = new(false);
        
        public readonly ReactiveProperty<float> FilterPanelWidth = new(150f);

        public readonly ReactiveProperty<bool> ShowExpressionPanel = new(false);
        public readonly ReactiveProperty<float> ExpressionPanelHeight = new(150f);
    }    
}
