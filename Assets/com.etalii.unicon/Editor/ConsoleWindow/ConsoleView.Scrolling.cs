namespace EtAlii.UniCon.Editor
{
    using RedMoon.ReactiveKit;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleView
    {
        private readonly Button _tailButton;
        
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
        
        private void ScrollWhenNeeded()
        {
            if (_viewModel.Settings.ScrollToTail)
            {
                _listViewScrollView.verticalScroller.value = _listViewScrollView.verticalScroller.highValue;
                _previousScrollValue = float.IsNaN(_listViewScrollView.contentRect.height) ? 0 : _listViewScrollView.verticalScroller.value;
            }
        }
    }    
}
