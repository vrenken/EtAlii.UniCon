namespace EtAlii.UniCon.Editor
{
    using System;
    using RedMoon.ReactiveKit;
    using Serilog.Events;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleView
    {
        private readonly Button _tailButton;

        private IDisposable _streamSubscription;

        private DateTimeOffset _skipNextToggleUntil;
        
        private void BindScrolling(ConsoleViewModel viewModel, CompositeDisposable disposable)
        {
            _tailButton
                .BindClick(viewModel.OnTailButtonClick)
                .AddTo(disposable);
            UpdateToggleButton(_tailButton, _viewModel.Settings.ScrollToTail);
        }

        private void OnScrollingChanged(string settingName)
        {
            switch (settingName)
            {
                case nameof(_viewModel.Settings.ScrollToTail):
                    UpdateToggleButton(_tailButton, _viewModel.Settings.ScrollToTail);
                    ScrollWhenNeeded();
                    break;
            }
        }
        private void OnStreamChanged()
        {
            if (_streamSubscription != null)
            {
                _streamSubscription.Dispose();
                _streamSubscription = null;
            }
            
            _items.Clear();
            _listView.Rebuild();

            // _previousScrollValue = _listViewScrollView.verticalScroller.value = 0f;
            
            _streamSubscription = _viewModel.Stream
                .Subscribe(onNext: Add);
        }

        private void Add(LogEvent logEvent)
        {
            _items.Add(logEvent);
            _listView.RefreshItems();

            ScrollWhenNeeded();
        }

        private void ScrollWhenNeeded()
        {
            if (_viewModel.Settings.ScrollToTail)
            {
                _listViewScrollView.verticalScroller.value = _listViewScrollView.verticalScroller.highValue;
                _previousScrollValue = float.IsNaN(_listViewScrollView.contentRect.height) ? 0 : _listViewScrollView.verticalScroller.value;
            }
            // else if (_scrollToHead)
            // {
            //     _listViewScrollView.verticalScroller.value = 0;
            //     _previousScrollValue = 0;
            //     _scrollToHead = false;
            // }
        }
    }    
}
