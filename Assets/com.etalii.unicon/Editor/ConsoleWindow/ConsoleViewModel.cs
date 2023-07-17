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
        public readonly ObservableCollection<LogEventLineViewModel> LogEvents = new();

        private ObservableCollection<LogEvent> _logEventsSource = new ();

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
            _logger.Information("Started Serilog logging");
            _logger.Information("Started UniCon Console");
        }

        public void Update()
        {
            if (LogSink.LogEvents != _logEventsSource)
            {
                _logEventsSource.CollectionChanged -= OnSourceChanged;
                _logEventsSource = LogSink.LogEvents;
                _logEventsSource.CollectionChanged += OnSourceChanged;
            }
        }

        private void OnSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var viewModel = CreateInstance<LogEventLineViewModel>();
                        viewModel.Init((LogEvent)item); 
                        LogEvents.Add(viewModel);
                    }
                    break;
            }
        }
    }    
}
