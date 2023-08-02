namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Linq;
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
        [SerializeField] private bool useUnitySource = true;

        /// <summary>
        /// Set this value to true to show all log events that are gathered by the default Microsoft loggers. 
        /// </summary>
        public readonly ReactiveProperty<bool> UseMicrosoftSource = new();
        [SerializeField] private bool useMicrosoftSource = true;

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
        
        public LogFilter[] CustomFilters { get => _customFiltersArray; set { _customFiltersArray = value; SaveLogFilters(); } }
        private LogFilter[] _customFiltersArray = Array.Empty<LogFilter>();
        [SerializeField] private string[] customFilters = Array.Empty<string>();

        public void Bind()
        {
#if UNICON_LIFETIME_DEBUG            
            Debug.Log($"[UNICON] {GetType().Name}.{nameof(Bind)}()", this);

            if (_disposable.Count > 0) throw new InvalidOperationException($"{GetType().Name} already bound");
#endif
            LogLevel.Value = logLevel;
            ShowExceptions.Value = showExceptions;
            
            UseSerilogSource.Value = useSerilogSource;
            UseUnitySource.Value = useUnitySource;
            UseMicrosoftSource.Value = useMicrosoftSource;
            
            ScrollToTail.Value = scrollToTail;
            ShowFilterPanel.Value = showFilterPanel;
            FilterPanelWidth.Value = filterPanelWidth;
            ShowExpressionPanel.Value = showExpressionPanel;
            ExpressionPanelHeight.Value = expressionPanelHeight;
            ClearOnPlay.Value = clearOnPlay;
            ClearOnBuild.Value = clearOnBuild;
            ClearOnRecompile.Value = clearOnRecompile;
            
            LogLevel.Subscribe(value => logLevel = value ).AddTo(_disposable);
            ShowExceptions.Subscribe(value => showExceptions = value ).AddTo(_disposable);
            
            UseSerilogSource.Subscribe(value => useSerilogSource = value ).AddTo(_disposable);
            UseUnitySource.Subscribe(value => useUnitySource = value ).AddTo(_disposable);
            UseMicrosoftSource.Subscribe(value => useMicrosoftSource = value ).AddTo(_disposable);
            
            ScrollToTail.Subscribe(value => scrollToTail = value ).AddTo(_disposable);
            ShowFilterPanel.Subscribe(value => showFilterPanel = value ).AddTo(_disposable);
            FilterPanelWidth.Subscribe(value => filterPanelWidth = value ).AddTo(_disposable);
            ShowExpressionPanel.Subscribe(value => showExpressionPanel = value ).AddTo(_disposable);
            ExpressionPanelHeight.Subscribe(value => expressionPanelHeight = value ).AddTo(_disposable);
            ClearOnPlay.Subscribe(value => clearOnPlay = value ).AddTo(_disposable);
            ClearOnBuild.Subscribe(value => clearOnBuild = value ).AddTo(_disposable);
            ClearOnRecompile.Subscribe(value => clearOnRecompile = value ).AddTo(_disposable);

            _customFiltersArray = LoadLogFilters();

            Observable
                .Merge(new[]
                {
                    LogLevel.Select(_ => true),
                    ShowExceptions.Select(_ => true),
                    UseSerilogSource.Select(_ => true),
                    UseUnitySource.Select(_ => true),
                    UseMicrosoftSource.Select(_ => true),
                    ScrollToTail.Select(_ => true),
                    ShowFilterPanel.Select(_ => true),
                    FilterPanelWidth.Select(_ => true),
                    ShowExpressionPanel.Select(_ => true),
                    ExpressionPanelHeight.Select(_ => true),
                    ClearOnPlay.Select(_ => true),
                    ClearOnBuild.Select(_ => true),
                    ClearOnRecompile.Select(_ => true),
                })
                .Throttle(_throttle)
                .Subscribe(_ => SaveWhenNeeded())
                .AddTo(_disposable);
        }

        public void SaveWhenNeeded()
        {
            if (!EditorUtility.IsPersistent(this))
            {
                Save(true);
            }
        }

        private void SaveLogFilters()
        {
            customFilters = _customFiltersArray
                .Select(f => f.Serialize())
                .ToArray();
        
            SaveWhenNeeded(); 
        }

        private LogFilter[] LoadLogFilters()
        {
            return customFilters
                .Select(f =>
                {
                    var filter = new LogFilter();
                    filter.Deserialize(f);
                    return filter;
                })
                .ToArray();
        }
    }    
}
