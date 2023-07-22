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
                if(_instance == null) _instance = CreateInstance<ConsoleViewModel>();
                return _instance;
            }
        }
        
        public readonly ConsoleSettings Settings = new();

        private static ConsoleViewModel _instance;
        
        private IDisposable _logEventsSource;

        public ConsoleViewModel()
        {
            SetupScrolling();
            SetupFilter();
            SetupExpression();
        }

        public void Init() => ConfigureStream();
    }    
}
