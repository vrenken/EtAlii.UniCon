namespace EtAlii.UniCon.Editor
{
    using System;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleViewModel
    {
        /// <summary>
        /// Gets raised when any filter related property has changed. 
        /// </summary>
        public event Action<string> FilterChanged;
        
        public readonly ReactiveCommand<ClickEvent> OnFilterButtonClick = new();

        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelSerilogToggleChange = new();
        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelUnityToggleChange = new();

        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelVerboseToggleChange = new();
        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelDebugToggleChange = new();
        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelInformationToggleChange = new();
        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelWarningToggleChange = new();
        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelErrorToggleChange = new();
        public readonly ReactiveCommand<ChangeEvent<bool>> OnLogLevelFatalToggleChange = new();

        public readonly ConsoleSettings Settings = new();

        private void SetupFilter()
        {
            OnFilterButtonClick.Subscribe(_ =>
            {
                Settings.ShowFilter = !Settings.ShowFilter;
                FilterChanged?.Invoke(nameof(Settings.ShowFilter));
            });

            OnLogLevelSerilogToggleChange.Subscribe(e => ToggleSource(nameof(Settings.UseSerilogSource), e.newValue, value => Settings.UseSerilogSource = value));
            OnLogLevelUnityToggleChange.Subscribe(e => ToggleSource(nameof(Settings.UseUnitySource), e.newValue, value => Settings.UseUnitySource = value));
                
            OnLogLevelVerboseToggleChange.Subscribe(e => ToggleLogLevel(nameof(Settings.LogLevel), e.newValue, LogLevel.Verbose, () => Settings.LogLevel, logLevel => Settings.LogLevel = logLevel));
            OnLogLevelDebugToggleChange.Subscribe(e => ToggleLogLevel(nameof(Settings.LogLevel), e.newValue, LogLevel.Debug, () => Settings.LogLevel, logLevel => Settings.LogLevel = logLevel));
            OnLogLevelInformationToggleChange.Subscribe(e => ToggleLogLevel(nameof(Settings.LogLevel), e.newValue, LogLevel.Information, () => Settings.LogLevel, logLevel => Settings.LogLevel = logLevel));
            OnLogLevelWarningToggleChange.Subscribe(e => ToggleLogLevel(nameof(Settings.LogLevel), e.newValue, LogLevel.Warning, () => Settings.LogLevel, logLevel => Settings.LogLevel = logLevel));
            OnLogLevelErrorToggleChange.Subscribe(e => ToggleLogLevel(nameof(Settings.LogLevel), e.newValue, LogLevel.Error, () => Settings.LogLevel, logLevel => Settings.LogLevel = logLevel));
            OnLogLevelFatalToggleChange.Subscribe(e => ToggleLogLevel(nameof(Settings.LogLevel), e.newValue, LogLevel.Fatal, () => Settings.LogLevel, logLevel => Settings.LogLevel = logLevel));
        }

        private void ToggleSource(string settingName, bool active, Action<bool> setSource)
        {
            setSource(active);

            FilterChanged?.Invoke(settingName);
            ConfigureStream();
        }
        
        private void ToggleLogLevel(string settingName, bool active, LogLevel value, Func<LogLevel> getLogLevel, Action<LogLevel> setLoglevel)
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
            
            FilterChanged?.Invoke(settingName);
            ConfigureStream();
        }
    }    
}
