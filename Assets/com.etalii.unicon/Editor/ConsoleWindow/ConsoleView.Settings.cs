namespace EtAlii.UniCon.Editor
{
    using RedMoon.ReactiveKit;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleView
    {
        private bool _isTrackingTail;

        private void OnTailButtonClicked()
        {
            _isTrackingTail = !_isTrackingTail;
            _tailButton.style.backgroundColor = _isTrackingTail ? _tailButtonTrackingColor : _tailButtonNonTrackingColor;
            _previousScrollValue = _listViewScrollView.verticalScroller.value;
        }

        private void BindSettings(ConsoleViewModel viewModel, CompositeDisposable disposable)
        {
            
            var verboseToggle = this.Q<Toggle>("verbose-toggle");
            verboseToggle.value = viewModel.Settings.LogLevel.HasFlag(LogLevel.Verbose);
            verboseToggle
                .BindValueChanged(viewModel.OnLogLevelVerboseToggleChange)
                .AddTo(disposable); 
            
            var informationToggle = this.Q<Toggle>("information-toggle");
            informationToggle.value = viewModel.Settings.LogLevel.HasFlag(LogLevel.Information);
            informationToggle
                .BindValueChanged(viewModel.OnLogLevelInformationToggleChange)
                .AddTo(disposable); 

            var debugToggle = this.Q<Toggle>("debug-toggle");
            debugToggle.value = viewModel.Settings.LogLevel.HasFlag(LogLevel.Debug);
            debugToggle
                .BindValueChanged(viewModel.OnLogLevelDebugToggleChange)
                .AddTo(disposable); 
            
            var warningToggle = this.Q<Toggle>("warning-toggle");
            warningToggle.value = viewModel.Settings.LogLevel.HasFlag(LogLevel.Warning);
            warningToggle
                .BindValueChanged(viewModel.OnLogLevelWarningToggleChange)
                .AddTo(disposable); 
            
            var errorToggle = this.Q<Toggle>("error-toggle");
            errorToggle.value = viewModel.Settings.LogLevel.HasFlag(LogLevel.Error);
            errorToggle
                .BindValueChanged(viewModel.OnLogLevelErrorToggleChange)
                .AddTo(disposable); 
            
            var fatalToggle = this.Q<Toggle>("fatal-toggle");
            fatalToggle.value = viewModel.Settings.LogLevel.HasFlag(LogLevel.Fatal);
            fatalToggle
                .BindValueChanged(viewModel.OnLogLevelFatalToggleChange)
                .AddTo(disposable); 


        }
    }    
}
