namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Linq;
    using RedMoon.ReactiveKit;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleView
    {
        private readonly Button _filterButton;
        private readonly VisualElement _filterPanel;
        private readonly Foldout _customFiltersFoldout;

        private void BindFilter(ConsoleViewModel viewModel, CompositeDisposable disposable)
        {
            _filterButton
                .BindClick(viewModel.ToggleFilterPanel)
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
                .BindTwoWayValueChanged(viewModel.Settings.LogLevel, LogLevel.Warning)
                .AddTo(disposable);
            
            var errorToggle = this.Q<Toggle>("error-toggle");
            errorToggle
                .BindTwoWayValueChanged(viewModel.Settings.LogLevel, LogLevel.Error)
                .AddTo(disposable);
 
            
            var fatalToggle = this.Q<Toggle>("fatal-toggle");
            fatalToggle
                .BindTwoWayValueChanged(viewModel.Settings.LogLevel, LogLevel.Fatal)
                .AddTo(disposable);

            var exceptionsToggle = this.Q<Toggle>("exceptions-toggle");
            exceptionsToggle
                .BindTwoWayValueChanged(viewModel.Settings.ShowExceptions)
                .AddTo(disposable);

            _viewModel.CustomFilters
                .ObserveAdd()
                .Subscribe(AddCustomFilter)
                .AddTo(disposable);
            _viewModel.CustomFilters
                .ObserveRemove()
                .Subscribe(RemoveCustomFilter)
                .AddTo(disposable);
            _viewModel.CustomFilters
                .ObserveReset()
                .Subscribe(ResetAllCustomFilters)
                .AddTo(disposable);
        }

        private void AddCustomFilter(CollectionAddEvent<CustomFilter> evt)
        {
            var disposables = new CompositeDisposable();
            var customFilter = evt.Value;
            var customFilterView = new Toggle
            {
                text = customFilter.Name,
                name = customFilter.Name,
                focusable = false,
                userData = new Tuple<CustomFilter, CompositeDisposable>(customFilter, disposables)
            };
            customFilterView
                .BindTwoWayValueChanged(customFilter.IsActive)
                .AddTo(disposables);
                
            _customFiltersFoldout.contentContainer.Add(customFilterView);
        }

        private void RemoveCustomFilter(CollectionRemoveEvent<CustomFilter> evt)
        {
            var customFilter = evt.Value;
            var (view, _, disposables) = _customFiltersFoldout.contentContainer
                .Children()
                .Select(v =>
                {
                    var (cf, disposables) = (Tuple<CustomFilter, CompositeDisposable>)v.userData ;
                    return (v, cf, disposables);
                })
                .Single(c => c.cf == customFilter);
            _customFiltersFoldout.contentContainer.Remove(view);
            disposables.Dispose();
        }

        private void ResetAllCustomFilters(Unit _)
        {
            var visibleCustomFilters = _customFiltersFoldout.contentContainer
                .Children()
                .Select(c => c.userData)
                .Cast<CustomFilter>()
                .ToArray();
            foreach (var customFilter in visibleCustomFilters)
            {
                RemoveCustomFilter(new CollectionRemoveEvent<CustomFilter>(-1, customFilter));
            }

            for (var i = 0; i < _viewModel.CustomFilters.Count; i++)
            {
                AddCustomFilter(new CollectionAddEvent<CustomFilter>(i, _viewModel.CustomFilters[i]));
            }
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
