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
#if UNICON_LIFETIME_DEBUG            
                    Debug.Log($"[UNICON] {nameof(ConsoleViewModel)}.{nameof(Instance)}");
#endif
                    _instance = CreateInstance<ConsoleViewModel>();
                }
                return _instance;
            }
        }

        private static ConsoleViewModel _instance;

        public StreamingViewModel Streaming => _streaming;
        private readonly StreamingViewModel _streaming = new ();

        public ExpressionViewModel Expressions => _expressions;
        private readonly ExpressionViewModel _expressions = new ();

        public FiltersViewModel Filters => _filters;
        private readonly FiltersViewModel _filters = new ();
        
        private IDisposable _logEventsSource;

        private void Awake()
        {
#if UNICON_LIFETIME_DEBUG            
            Debug.Log($"[UNICON] {GetType().Name}.{nameof(Awake)}()");
#endif
            UserSettings.instance.Bind();
            ProjectSettings.instance.Bind();

            _filters.Bind(_expressions, _streaming);
            _expressions.Bind(_filters, _streaming);
            _streaming.Bind(_filters, _expressions);
        }

        public void Init() => _streaming.ConfigureStream();
        
        public void Clear()
        {
            LogSink.Instance.Clear();
            _streaming.ConfigureStream();
        }
    }    
}
