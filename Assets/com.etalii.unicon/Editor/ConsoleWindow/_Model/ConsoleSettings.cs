namespace EtAlii.UniCon.Editor
{
    public class ConsoleSettings
    {
        public LogLevel LogLevel = LogLevel.Information | LogLevel.Debug | LogLevel.Warning | LogLevel.Error | LogLevel.Fatal;

        public bool UseSerilogSource = true;
        public bool UseUnitySource = false;

        public bool ScrollToTail = true;

        public bool ShowFilter = true;
        public bool ShowRules = false;
    }    
}
