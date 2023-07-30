namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Linq;
    using RedMoon.ReactiveKit;
    using UniRx;
    using UnityEngine.UIElements;

    public class FiltersView
    {
        private readonly TwoPaneSplitView _horizontalSplitPanel;
        private readonly Button _filterButton;
        private readonly VisualElement _filterPanel;
        private readonly Foldout _customFiltersFoldout;
        private readonly Toggle _serilogSourceToggle;
        private readonly Toggle _unitySourceToggle;
        private readonly Toggle _verboseToggle;
        private readonly Toggle _informationToggle;
        private readonly Toggle _debugToggle;
        private readonly Toggle _warningToggle;
        private readonly Toggle _errorToggle;
        private readonly Toggle _fatalToggle;
        private readonly Toggle _exceptionsToggle;
        private FiltersViewModel _viewModel;

        public FiltersView(VisualElement root)
        {
            _horizontalSplitPanel = root.Q<TwoPaneSplitView>("horizontal-split-panel");

            _filterPanel = root.Q<VisualElement>("filter-panel");
            _filterButton = root.Q<Button>("filter-button");
            _customFiltersFoldout = root.Q<Foldout>("custom-filters-foldout");
            
            _serilogSourceToggle = root.Q<Toggle>("serilog-source-toggle");
            _unitySourceToggle = root.Q<Toggle>("unity-source-toggle");
            _verboseToggle = root.Q<Toggle>("verbose-toggle");
            _informationToggle = root.Q<Toggle>("information-toggle");
            _debugToggle = root.Q<Toggle>("debug-toggle");
            _warningToggle = root.Q<Toggle>("warning-toggle");
            _errorToggle = root.Q<Toggle>("error-toggle");
            _fatalToggle = root.Q<Toggle>("fatal-toggle");
            _exceptionsToggle = root.Q<Toggle>("exceptions-toggle");
        }
        
        public void Bind(FiltersViewModel viewModel, CompositeDisposable disposable)
        {
            _viewModel = viewModel;
            
            _filterButton
                .BindClick(_viewModel.ToggleFilterPanel)
                .AddTo(disposable);
            _viewModel.UserSettings.ShowFilterPanel
                .Subscribe(showFilterPanel =>
                {
                    UpdateFilterPanel();
                    _filterButton.UpdateToggleButton(showFilterPanel);
                })
                .AddTo(disposable);
            _filterPanel
                .BindCallback<GeometryChangedEvent>(_ =>
                {
                    if (_filterPanel.visible && _filterPanel.contentRect.width > 0)
                    {
                        _viewModel.UserSettings.FilterPanelWidth.Value = _filterPanel.contentRect.width;
                    }
                })
                .AddTo(disposable);
            _viewModel.UserSettings.FilterPanelWidth
                .Throttle(TimeSpan.FromMilliseconds(300))
                .Subscribe(_ => UpdateFilterPanel())
                .AddTo(disposable);
            
            // Log sources.
            _serilogSourceToggle
                .BindTwoWayValueChanged(_viewModel.UserSettings.UseSerilogSource)
                .AddTo(disposable);
            
            _unitySourceToggle
                .BindTwoWayValueChanged(_viewModel.UserSettings.UseUnitySource)
                .AddTo(disposable);

            // Log levels.
            _verboseToggle
                .BindTwoWayValueChanged(viewModel.UserSettings.LogLevel, LogLevel.Verbose)
                .AddTo(disposable);

            _informationToggle
                .BindTwoWayValueChanged(viewModel.UserSettings.LogLevel, LogLevel.Information)
                .AddTo(disposable);

            _debugToggle
                .BindTwoWayValueChanged(viewModel.UserSettings.LogLevel, LogLevel.Debug)
                .AddTo(disposable);

            _warningToggle
                .BindTwoWayValueChanged(viewModel.UserSettings.LogLevel, LogLevel.Warning)
                .AddTo(disposable);
            
            _errorToggle
                .BindTwoWayValueChanged(viewModel.UserSettings.LogLevel, LogLevel.Error)
                .AddTo(disposable);
 
            _fatalToggle
                .BindTwoWayValueChanged(viewModel.UserSettings.LogLevel, LogLevel.Fatal)
                .AddTo(disposable);

            _exceptionsToggle
                .BindTwoWayValueChanged(viewModel.UserSettings.ShowExceptions)
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

        public void Unbind()
        {
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
            _filterPanel.visible = _viewModel.UserSettings.ShowFilterPanel.Value;
            
            var width = _filterPanel.visible
                ? _viewModel.UserSettings.FilterPanelWidth.Value
                : 0f;
            _horizontalSplitPanel.fixedPaneInitialDimension = width;
            _filterPanel.style.width = width;
        }
    }    
}
