namespace EtAlii.UniCon.Editor
{
    using RedMoon.ReactiveKit;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleView
    {
        private readonly Button _filterButton;
        private readonly VisualElement _filterPanel;

        private void BindFilter(ConsoleViewModel viewModel, CompositeDisposable disposable)
        {
            _filterButton
                .BindClick(viewModel.OnFilterButtonClick)
                .AddTo(disposable);
            _viewModel.Settings.ShowFilterPanel
                .Subscribe(onNext: showFilterPanel =>
                {
                    UpdateFilterPanel();
                    UpdateToggleButton(_filterButton, showFilterPanel);
                })
                .AddTo(disposable);
            _viewModel.Settings.FilterPanelWidth
                .Subscribe(filterPanelWidth =>
                {
                    var width = _filterPanel.visible
                        ? filterPanelWidth
                        : 0f;
                    _horizontalSplitPanel.fixedPaneInitialDimension = width;
                    _filterPanel.style.width = width;
                })
                .AddTo(disposable);
            
            // Log sources.
            var serilogSourceToggle = this.Q<Toggle>("serilog-source-toggle");
            serilogSourceToggle
                .BindTwoWayValueChanged(viewModel.Settings.UseSerilogSource)
                .AddTo(disposable);
            
            var unitySourceToggle = this.Q<Toggle>("unity-source-toggle");
            unitySourceToggle
                .BindTwoWayValueChanged(viewModel.Settings.UseUnitySource)
                .AddTo(disposable);

            // Log levels.
            var verboseToggle = this.Q<Toggle>("verbose-toggle");
            verboseToggle
                .BindTwoWayValueChanged(viewModel.Settings.LogLevel, LogLevel.Verbose)
                .AddTo(disposable);

            var informationToggle = this.Q<Toggle>("information-toggle");
            informationToggle
                .BindTwoWayValueChanged(viewModel.Settings.LogLevel, LogLevel.Information)
                .AddTo(disposable);

            var debugToggle = this.Q<Toggle>("debug-toggle");
            debugToggle
                .BindTwoWayValueChanged(viewModel.Settings.LogLevel, LogLevel.Debug)
                .AddTo(disposable);

            
            var warningToggle = this.Q<Toggle>("warning-toggle");
            warningToggle
                .BindTwoWayValueChanged(viewModel.Settings.LogLevel, LogLevel.Error)
                .AddTo(disposable);
            
            var errorToggle = this.Q<Toggle>("error-toggle");
            errorToggle
                .BindTwoWayValueChanged(viewModel.Settings.LogLevel, LogLevel.Warning)
                .AddTo(disposable);
 
            
            var fatalToggle = this.Q<Toggle>("fatal-toggle");
            fatalToggle
                .BindTwoWayValueChanged(viewModel.Settings.LogLevel, LogLevel.Fatal)
                .AddTo(disposable);

            
            var exceptionsToggle = this.Q<Toggle>("exceptions-toggle");
            exceptionsToggle
                .BindTwoWayValueChanged(viewModel.Settings.ShowExceptions)
                .AddTo(disposable);
        }
        
        private void UpdateFilterPanel()
        {
            if (_filterPanel.visible)
            {
                _viewModel.Settings.FilterPanelWidth.Value = _filterPanel.contentRect.width > 0f 
                    ? _filterPanel.contentRect.width 
                    : 150;
            }
            _filterPanel.visible = _viewModel.Settings.ShowFilterPanel.Value;
        }
    }    
}
