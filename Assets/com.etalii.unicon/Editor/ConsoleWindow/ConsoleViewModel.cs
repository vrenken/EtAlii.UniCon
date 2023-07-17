namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Collections.ObjectModel;
    using Serilog;
    using Serilog.Sinks.UniCon;
    using UniRx;
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

        // ReSharper disable once CollectionNeverQueried.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public readonly ObservableCollection<LogEventViewModel> LogEvents = new();

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
        }

        public void Init()
        {
            _logEventsSource = LogSink.Instance
                .Observe()
                .SubscribeOn(Scheduler.Immediate)
                .Subscribe(onNext: logEvent =>
                {
                    var viewModel = CreateInstance<LogEventViewModel>();
                    viewModel.Init(logEvent); 
                    LogEvents.Add(viewModel);
                });
        }
    }    
}
