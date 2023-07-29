namespace EtAlii.UniCon.Editor
{
    using System;
    using UniRx;
    using UnityEditor;
    using UnityEngine;

    [FilePath("UserSettings/UniConSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    internal class UserSettings : ScriptableSingleton<UserSettings>
    {
        /// <summary>
        /// The log level(s) that should be visualized. if no log levels are specified no filtering on log levels will happen.  
        /// </summary>
        public readonly ReactiveProperty<LogLevel> LogLevel = new();
        [SerializeField] private LogLevel logLevel = EtAlii.UniCon.Editor.LogLevel.None;
        
        /// <summary>
        /// Set this value to true to show any log event that contains information about an exception. This overrules the default log levels.
        /// </summary>
        public readonly ReactiveProperty<bool> ShowExceptions = new();
        [SerializeField] private bool showExceptions;
        
        /// <summary>
        /// Set this value to true to show all log events that are gathered by Serilog. 
        /// </summary>
        public readonly ReactiveProperty<bool> UseSerilogSource = new();
        [SerializeField] private bool useSerilogSource = true;

        /// <summary>
        /// Set this value to true to show all log events that are gathered by the default Unity debug logs. 
        /// </summary>
        public readonly ReactiveProperty<bool> UseUnitySource = new();
        [SerializeField] private bool useUnitySource;

        /// <summary>
        /// Set this property to true to keep scrolling to any new log events received.
        /// </summary>
        public readonly ReactiveProperty<bool> ScrollToTail = new();
        [SerializeField] private bool scrollToTail = true;

        public readonly ReactiveProperty<bool> ShowFilterPanel = new();
        [SerializeField] private bool showFilterPanel;
        
        public readonly ReactiveProperty<float> FilterPanelWidth = new();
        [SerializeField] private float filterPanelWidth = 150f;

        public readonly ReactiveProperty<bool> ShowExpressionPanel = new();
        [SerializeField] private bool showExpressionPanel;

        public readonly ReactiveProperty<float> ExpressionPanelHeight = new();
        [SerializeField] private float expressionPanelHeight = 150f;

        public readonly ReactiveProperty<bool> ClearOnPlay = new();
        [SerializeField] private bool clearOnPlay = true;

        public readonly ReactiveProperty<bool> ClearOnBuild = new();
        [SerializeField] private bool clearOnBuild = true;

        public readonly ReactiveProperty<bool> ClearOnRecompile = new();
        [SerializeField] private bool clearOnRecompile = true;

        // ReSharper disable once NotAccessedField.Local
        private readonly CompositeDisposable _disposable = new ();

        private readonly TimeSpan _throttle = TimeSpan.FromMilliseconds(100);

        public void Bind()
        {
#if UNICON_LIFETIME_DEBUG            
            Debug.Log($"STARTUP: {GetType().Name}.{nameof(Bind)}()");
#endif
            LogLevel.Throttle(_throttle).Subscribe(value => { logLevel = value; SaveWhenNeeded(); }).AddTo(_disposable);
            ShowExceptions.Throttle(_throttle).Subscribe(value => { showExceptions = value; SaveWhenNeeded(); }).AddTo(_disposable);
            UseSerilogSource.Throttle(_throttle).Subscribe(value => { useSerilogSource = value; SaveWhenNeeded(); }).AddTo(_disposable);
            UseUnitySource.Throttle(_throttle).Subscribe(value => { useUnitySource = value; SaveWhenNeeded(); }).AddTo(_disposable);
            ScrollToTail.Throttle(_throttle).Subscribe(value => { scrollToTail = value; SaveWhenNeeded(); }).AddTo(_disposable);
            ShowFilterPanel.Throttle(_throttle).Subscribe(value => { showFilterPanel = value; SaveWhenNeeded(); }).AddTo(_disposable);
            FilterPanelWidth.Throttle(_throttle).Subscribe(value => { filterPanelWidth = value; SaveWhenNeeded(); }).AddTo(_disposable);
            ShowExpressionPanel.Throttle(_throttle).Subscribe(value => { showExpressionPanel = value; SaveWhenNeeded(); }).AddTo(_disposable);
            ExpressionPanelHeight.Throttle(_throttle).Subscribe(value => { expressionPanelHeight = value; SaveWhenNeeded(); }).AddTo(_disposable);
            ClearOnPlay.Throttle(_throttle).Subscribe(value => { clearOnPlay = value; SaveWhenNeeded(); }).AddTo(_disposable);
            ClearOnBuild.Throttle(_throttle).Subscribe(value => { clearOnBuild = value; SaveWhenNeeded(); }).AddTo(_disposable);
            ClearOnRecompile.Throttle(_throttle).Subscribe(value => { clearOnRecompile = value; SaveWhenNeeded(); }).AddTo(_disposable);

            LogLevel.SetValueAndForceNotify(logLevel);
            ShowExceptions.SetValueAndForceNotify(showExceptions);
            UseSerilogSource.SetValueAndForceNotify(useSerilogSource);
            UseUnitySource.SetValueAndForceNotify(useUnitySource);
            ScrollToTail.SetValueAndForceNotify(scrollToTail);
            ShowFilterPanel.SetValueAndForceNotify(showFilterPanel);
            FilterPanelWidth.SetValueAndForceNotify(filterPanelWidth);
            ShowExpressionPanel.SetValueAndForceNotify(showExpressionPanel);
            ExpressionPanelHeight.SetValueAndForceNotify(expressionPanelHeight);
            ClearOnPlay.SetValueAndForceNotify(clearOnPlay);
            ClearOnBuild.SetValueAndForceNotify(clearOnBuild);
            ClearOnRecompile.SetValueAndForceNotify(clearOnRecompile);
        }

        private void SaveWhenNeeded()
        {
            if (!EditorUtility.IsPersistent(this))
            {
                Save(true);
            }
        }
    }    
}
