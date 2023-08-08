namespace EtAlii.UniCon.Editor
{
    using System;
    using UnityEngine;

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

        public StreamingViewModel Streaming => _streaming;
        private readonly StreamingViewModel _streaming;

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
        public DataWindowStreamer DataStreamer => _dataWindowStreamer;
        private readonly DataWindowStreamer _dataWindowStreamer;
        
        private IDisposable _logEventsSource;

        public ConsoleViewModel()
        {
            _dataWindowStreamer = new DataWindowStreamer();
            _filters = new(_dataWindowStreamer);
            _streaming = new StreamingViewModel();
            _expressions = new (_dataWindowStreamer);
        }
        
        private void Awake()
        {
#if UNICON_LIFETIME_DEBUG            
            Debug.Log($"[UNICON] {GetType().Name}.{nameof(Awake)}()", this);
#endif
            UserSettings.instance.Bind();
            ProjectSettings.instance.Bind();

            _dataWindowStreamer.Bind(_expressions);
            _streaming.Bind();
            _expressions.Bind();
            _filters.Bind(_expressions);
        }

        public void Initialize() => _dataWindowStreamer.Configure();
        
        public void Clear()
        {
            LogSink.Instance.Clear();
            _dataWindowStreamer.Configure();
        }
    }    
}
