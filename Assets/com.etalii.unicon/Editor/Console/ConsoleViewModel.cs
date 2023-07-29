namespace EtAlii.UniCon.Editor
{
    using System;
    using UnityEngine;

    public partial class ConsoleViewModel : ScriptableObject
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
        
        internal UserSettings UserSettings => UserSettings.instance;
        internal ProjectSettings ProjectSettings => ProjectSettings.instance;

        private static ConsoleViewModel _instance;
        
        private IDisposable _logEventsSource;

        private void Awake()
        {
#if UNICON_LIFETIME_DEBUG            
            Debug.Log($"[UNICON] {GetType().Name}.{nameof(Awake)}()");
#endif
            SetupScrolling();
            SetupFilter();
            SetupExpression();
        }

        public void Init() => ConfigureStream();
    }    
}
