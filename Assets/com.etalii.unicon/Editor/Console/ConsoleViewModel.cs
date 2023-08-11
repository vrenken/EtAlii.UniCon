namespace EtAlii.UniCon.Editor
{
    using System;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class ConsoleViewModel : ScriptableObject
    {
        public static ConsoleViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = CreateInstance<ConsoleViewModel>();
#if UNICON_LIFETIME_DEBUG            
                    Debug.Log($"[UNICON] {nameof(ConsoleViewModel)}.{nameof(Instance)}", _instance);
#endif
                }
                return _instance;
            }
        }

        private static ConsoleViewModel _instance;

        /// <summary>
        /// provides access to the expression view model - which assists with visualizing the expression panel below the log list.
        /// </summary>
        public ExpressionViewModel Expressions => _expressions;
        private readonly ExpressionViewModel _expressions;

        /// <summary>
        /// Provides access to the filters viewmodel. 
        /// </summary>
        public FiltersViewModel Filters => _filters;
        private readonly FiltersViewModel _filters;

        /// <summary>
        /// An instance of the data streamer class that is able to provide
        /// both forward and backward filtered streaming. 
        /// </summary>
        public DataStreamer DataStreamer => _dataStreamer;
        private readonly DataStreamer _dataStreamer;
        
        public readonly ReactiveCommand<ClickEvent> ToggleScrollToTail = new();

        private IDisposable _logEventsSource;

        public ConsoleViewModel()
        {
            _dataStreamer = new DataStreamer();
            _filters = new(_dataStreamer);
            _expressions = new (_dataStreamer);
        }
        
        private void Awake()
        {
#if UNICON_LIFETIME_DEBUG            
            Debug.Log($"[UNICON] {GetType().Name}.{nameof(Awake)}()", this);
#endif
            UserSettings.instance.Bind();
            ProjectSettings.instance.Bind();

            ToggleScrollToTail.Subscribe(_ =>
            {
                UserSettings.instance.ScrollToTail.Value = !UserSettings.instance.ScrollToTail.Value;
            });

            _dataStreamer.Bind(_expressions);
            _expressions.Bind();
            _filters.Bind(_expressions);
        }

        public void Initialize() => _dataStreamer.ConfigureHard();
        
        public void Clear()
        {
            LogSink.Instance.Clear();
            _dataStreamer.ConfigureHard();
        }
    }    
}
