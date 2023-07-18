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

            ScrollToHead();

            _streamSubscription = _viewModel.Stream
                .Subscribe(onNext: Add);
        }

        private void Add(LogEvent logEvent)
        {
            _items.Add(logEvent);
            _listView.RefreshItems();

            ScrollToTailWhenNeeded();
        }

        private void ScrollToHead()
        {
            _listViewScrollView.verticalScroller.value = 0;
            _previousScrollValue = 0;
        }

        private void ScrollToTailWhenNeeded()
        {
            if (_viewModel.Settings.ScrollToTail)
            {
                _listViewScrollView.verticalScroller.value = _listViewScrollView.verticalScroller.highValue > 0
                    ? _listViewScrollView.verticalScroller.highValue
                    : 0;
                _previousScrollValue = _listViewScrollView.contentContainer.childCount > 0 
                    ? _listViewScrollView.verticalScroller.value 
                    : 0f;
            }
        }
    }    
}
