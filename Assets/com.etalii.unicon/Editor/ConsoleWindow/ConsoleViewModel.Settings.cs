namespace EtAlii.UniCon.Editor
{
    using System;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleViewModel
    {
        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelSerilogToggleChange = new();
        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelUnityToggleChange = new();

        public readonly ReactiveCommand<ChangeEvent<bool>> OnSourceToggleChange = new();
        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelVerboseToggleChange = new();
        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelDebugToggleChange = new();
        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelInformationToggleChange = new();
        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelWarningToggleChange = new();
        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelErrorToggleChange = new();
        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelFatalToggleChange = new();

        public readonly ConsoleSettings Settings = new();

        private void SetupSettings()
        {
            OnLogLevelSerilogToggleChange.Subscribe(e => ToggleSource(e.newValue, value => Settings.UseSerilogSource = value));
            OnLogLevelUnityToggleChange.Subscribe(e => ToggleSource(e.newValue, value => Settings.UseUnitySource = value));
                
            OnLogLevelVerboseToggleChange.Subscribe(e => ToggleLogLevel(e.newValue, LogLevel.Verbose, () => Settings.LogLevel, logLevel => Settings.LogLevel = logLevel));
            OnLogLevelDebugToggleChange.Subscribe(e => ToggleLogLevel(e.newValue, LogLevel.Debug, () => Settings.LogLevel, logLevel => Settings.LogLevel = logLevel));
            OnLogLevelInformationToggleChange.Subscribe(e => ToggleLogLevel(e.newValue, LogLevel.Information, () => Settings.LogLevel, logLevel => Settings.LogLevel = logLevel));
            OnLogLevelWarningToggleChange.Subscribe(e => ToggleLogLevel(e.newValue, LogLevel.Warning, () => Settings.LogLevel, logLevel => Settings.LogLevel = logLevel));
            OnLogLevelErrorToggleChange.Subscribe(e => ToggleLogLevel(e.newValue, LogLevel.Error, () => Settings.LogLevel, logLevel => Settings.LogLevel = logLevel));
            OnLogLevelFatalToggleChange.Subscribe(e => ToggleLogLevel(e.newValue, LogLevel.Fatal, () => Settings.LogLevel, logLevel => Settings.LogLevel = logLevel));
        }

        private void ToggleSource(bool active, Action<bool> setSource)
        {
            setSource(active);
            ConfigureStream();
        }
        
        private void ToggleLogLevel(bool active, LogLevel value, Func<LogLevel> getLogLevel, Action<LogLevel> setLoglevel)
        {
            var logLevel = getLogLevel();
            if (active)
            {
                logLevel |= value;
            }
            else
            {
                logLevel &= ~value;
            }

            setLoglevel(logLevel);
            
            ConfigureStream();
        }
    }    
}
