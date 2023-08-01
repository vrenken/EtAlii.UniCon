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
        private readonly StreamingViewModel _streaming = new ();

        public ExpressionViewModel Expressions => _expressions;
        private readonly ExpressionViewModel _expressions = new ();

        public FiltersViewModel Filters => _filters;
        private readonly FiltersViewModel _filters = new ();
        
        private IDisposable _logEventsSource;

        private void Awake()
        {
#if UNICON_LIFETIME_DEBUG            
            Debug.Log($"[UNICON] {GetType().Name}.{nameof(Awake)}()", this);
#endif
            UserSettings.instance.Bind();
            ProjectSettings.instance.Bind();

            _expressions.Bind(_streaming);
            _filters.Bind(_expressions, _streaming);
            _streaming.Bind(_filters, _expressions);
        }

        public void Initialize() => _streaming.ConfigureStream();
        
        public void Clear()
        {
            LogSink.Instance.Clear();
            _streaming.ConfigureStream();
        }
    }    
}
