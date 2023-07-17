namespace EtAlii.UniCon.Editor
{
    using System;
    using UniRx;

    public partial class ConsoleViewModel
    {
        public IObservable<LogEventViewModel> Stream;
        public event Action StreamChanged;

        private void ConfigureStream()
        {
            Stream = LogSink.Instance
                .Observe()
                .Select(logEvent =>
                {
                    var viewModel = CreateInstance<LogEventViewModel>();
                    viewModel.Init(logEvent); 
                    return viewModel;
                });
            StreamChanged?.Invoke();            
        }
    }    
}
