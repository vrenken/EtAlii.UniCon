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
            UpdateToggleButton(_filterButton, _viewModel.Settings.ShowFilterPanel);
            UpdateFilterPanel();

            // Log sources.
            var serilogSourceToggle = this.Q<Toggle>("serilog-source-toggle");
            serilogSourceToggle.value = viewModel.Settings.UseSerilogSource;
            serilogSourceToggle
                .BindValueChanged(viewModel.OnLogLevelSerilogToggleChange)
                .AddTo(disposable); 
            
            var unitySourceToggle = this.Q<Toggle>("unity-source-toggle");
            unitySourceToggle.value = viewModel.Settings.UseUnitySource;
            unitySourceToggle
                .BindValueChanged(viewModel.OnLogLevelUnityToggleChange)
                .AddTo(disposable);

            // Log levels.
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
            
            var exceptionsToggle = this.Q<Toggle>("exceptions-toggle");
            exceptionsToggle.value = viewModel.Settings.ShowExceptions;
            exceptionsToggle
                .BindValueChanged(viewModel.OnShowExceptionsToggleChange)
                .AddTo(disposable);
        }
        
        private void OnFilterChanged(string settingName)
        {
            switch (settingName)
            {
                case nameof(_viewModel.Settings.ShowFilterPanel):
                    UpdateToggleButton(_filterButton, _viewModel.Settings.ShowFilterPanel);
                    UpdateFilterPanel();
                    break;
            }
        }

        private void UpdateFilterPanel()
        {
            if (_filterPanel.visible)
            {
                _viewModel.Settings.FilterPanelWidth = _filterPanel.contentRect.width > 0f 
                    ? _filterPanel.contentRect.width 
                    : 150;
            }

            _filterPanel.visible = _viewModel.Settings.ShowFilterPanel;
            var width = _filterPanel.visible
                ? _viewModel.Settings.FilterPanelWidth
                : 0f;
            _horizontalSplitPanel.fixedPaneInitialDimension = width;
            _filterPanel.style.width = width;
        }
    }    
}
