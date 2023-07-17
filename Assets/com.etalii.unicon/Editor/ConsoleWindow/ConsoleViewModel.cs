namespace EtAlii.UniCon.Editor
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using Serilog;
    using Serilog.Events;
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

        // ReSharper disable once CollectionNeverQueried.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public readonly ObservableCollection<LogEventViewModel> LogEvents = new();

        private readonly ObservableCollection<LogEvent> _logEventsSource;

        public ConsoleViewModel()
        {
            _logEventsSource = LogSink.Instance.LogEvents;
            _logEventsSource.CollectionChanged += OnSourceChanged;

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
            _logger.Information("Started Serilog logging");
            _logger.Information("Started UniCon Console");
        }

        // public void Init()
        // {
        // }

        private void OnSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var viewModel = CreateInstance<LogEventViewModel>();
                        viewModel.Init((LogEvent)item); 
                        LogEvents.Add(viewModel);
                    }
                    break;
            }
        }
    }    
}
