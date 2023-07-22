namespace EtAlii.UniCon.Editor
{
    using System;
    using Serilog.Events;
    using UniRx;

    public partial class ConsoleView
    {
        private IDisposable _streamSubscription;

        private void OnStreamChanged()
        {
            if (_streamSubscription != null)
            {
                _streamSubscription.Dispose();
                _streamSubscription = null;
            }

            _items.Clear();
            _listView.Rebuild();

            _streamSubscription = _viewModel.Stream
                .Subscribe(onNext: Add);
        }

        private void Add(LogEvent logEvent)
        {
            _items.Add(logEvent);
            _listView.RefreshItems();

            UpdateMetrics();
            ScrollWhenNeeded();
        }

        private void UpdateMetrics()
        {
            var total = LogSink.Instance.EventCount;
            var filtered = _items.Count;

            _metricsButton.text = $"Filtered: {Format(filtered)} / Total: {Format(total)}";
        }

        private string Format(int number)
        {
            return number switch
            {
                > 1000000 => $"{number / 1000000:D}M",
                > 1000 => $"{number / 1000:D}K",
                _ => $"{number}"
            };
        }
    }    
}
