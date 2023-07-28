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
            _viewModel.UserSettings.ScrollToTail
                .Subscribe(onNext: _ =>
                {
                    UpdateToggleButton(_tailButton, _viewModel.UserSettings.ScrollToTail.Value);
                    ScrollWhenNeeded();
                })
                .AddTo(disposable);

        }

        /// <summary>
        /// When tail tracking is active We need to check if the user scrolled upwards.
        /// In that case we stop the tail tracking.  
        /// </summary>
        /// <param name="value"></param>
        private void OnScrolledVertically(float value)
        {
            if (!_viewModel.UserSettings.ScrollToTail.Value) return;
            if (_previousScrollValue <= _listViewScrollView.verticalScroller.value) return;
            
            _viewModel.UserSettings.ScrollToTail.Value = false;
            UpdateToggleButton(_tailButton, _viewModel.UserSettings.ScrollToTail.Value);
        }

        private void ScrollWhenNeeded()
        {
            if (_viewModel.UserSettings.ScrollToTail.Value)
            {
                _listView.ScrollToItem(-1);
                _listViewScrollView.verticalScroller.value = _listViewScrollView.verticalScroller.highValue;
                _previousScrollValue = float.IsNaN(_listViewScrollView.contentRect.height) ? 0 : _listViewScrollView.verticalScroller.value;
            }
        }
    }    
}
