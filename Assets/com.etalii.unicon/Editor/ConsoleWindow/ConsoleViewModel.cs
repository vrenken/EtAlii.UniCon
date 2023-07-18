namespace EtAlii.UniCon.Editor
{
    using System;
    using Serilog;
    using Serilog.Sinks.UniCon;
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
        
        private static ConsoleViewModel _instance;
        private readonly Serilog.ILogger _logger;
        
        private IDisposable _logEventsSource;

        public ConsoleViewModel()
        {
            _originalLogHandler = Debug.unityLogger.logHandler;
            Debug.unityLogger.logHandler = this;

            if (Log.Logger == Serilog.Core.Logger.None)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.UniCon()
                    .CreateLogger();
            }

            _logger = Log.Logger;
            
            SetupSettings();
        }

        public void Init() => ConfigureStream();
    }    
}
