namespace EtAlii.UniCon.Editor
{
    using System;
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
                _listView.Clear();
            }
            _streamSubscription = _viewModel.Stream.Subscribe(onNext: Add);
        }

        private void Add(LogEventViewModel viewModel)
        {
            _items.Add(viewModel);
            _listView.RefreshItems();

            if (_isTrackingTail)
            {
                _listViewScrollView.verticalScroller.value = _listViewScrollView.verticalScroller.highValue > 0 ? _listViewScrollView.verticalScroller.highValue : 0;
                //_listViewScrollView.ScrollTo(_listViewScrollView...itemsSource[_listView.itemsSource.Count - 1]);
                //_listView.ScrollToItem(-1);
                //_listViewScrollView.verticalScroller.ScrollPageDown();
            }
        }

    }    
}
