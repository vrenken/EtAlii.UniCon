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
            UserSettings.instance.ShowFilterPanel
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
                        UserSettings.instance.FilterPanelWidth.Value = _filterPanel.contentRect.width;
                    }
                })
                .AddTo(disposable);
            UserSettings.instance.FilterPanelWidth
                .Throttle(TimeSpan.FromMilliseconds(300))
                .Subscribe(_ => UpdateFilterPanel())
                .AddTo(disposable);
            
            // Log sources.
            _serilogSourceToggle
                .BindTwoWayValueChanged(UserSettings.instance.UseSerilogSource)
                .AddTo(disposable);
            
            _unitySourceToggle
                .BindTwoWayValueChanged(UserSettings.instance.UseUnitySource)
                .AddTo(disposable);

            // Log levels.
            _verboseToggle
                .BindTwoWayValueChanged(UserSettings.instance.LogLevel, LogLevel.Verbose)
                .AddTo(disposable);

            _informationToggle
                .BindTwoWayValueChanged(UserSettings.instance.LogLevel, LogLevel.Information)
                .AddTo(disposable);

            _debugToggle
                .BindTwoWayValueChanged(UserSettings.instance.LogLevel, LogLevel.Debug)
                .AddTo(disposable);

            _warningToggle
                .BindTwoWayValueChanged(UserSettings.instance.LogLevel, LogLevel.Warning)
                .AddTo(disposable);
            
            _errorToggle
                .BindTwoWayValueChanged(UserSettings.instance.LogLevel, LogLevel.Error)
                .AddTo(disposable);
 
            _fatalToggle
                .BindTwoWayValueChanged(UserSettings.instance.LogLevel, LogLevel.Fatal)
                .AddTo(disposable);

            _exceptionsToggle
                .BindTwoWayValueChanged(UserSettings.instance.ShowExceptions)
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

            ResetAllCustomFilters(Unit.Default);
        }

        public void Unbind()
        {
        }

        private void AddCustomFilter(CollectionAddEvent<LogFilter> evt)
        {
            var disposables = new CompositeDisposable();
            var filter = evt.Value;
            var filterView = new Toggle
            {
                text = filter.Name.Value,
                name = filter.Name.Value,
                focusable = false,
                userData = new Tuple<LogFilter, CompositeDisposable>(filter, disposables)
            };
            filterView
                .BindTwoWayValueChanged(filter.IsActive)
                .AddTo(disposables);
                
            _customFiltersFoldout.contentContainer.Add(filterView);
        }

        private void RemoveCustomFilter(CollectionRemoveEvent<LogFilter> evt)
        {
            var customFilter = evt.Value;
            var (view, _, disposables) = _customFiltersFoldout.contentContainer
                .Children()
                .Select(v =>
                {
                    var (cf, disposables) = (Tuple<LogFilter, CompositeDisposable>)v.userData ;
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
                .Cast<LogFilter>()
                .ToArray();
            foreach (var customFilter in visibleCustomFilters)
            {
                RemoveCustomFilter(new CollectionRemoveEvent<LogFilter>(-1, customFilter));
            }

            for (var i = 0; i < _viewModel.CustomFilters.Count; i++)
            {
                AddCustomFilter(new CollectionAddEvent<LogFilter>(i, _viewModel.CustomFilters[i]));
            }
        }

        private void UpdateFilterPanel()
        {
            _filterPanel.visible = UserSettings.instance.ShowFilterPanel.Value;
            
            var width = _filterPanel.visible
                ? UserSettings.instance.FilterPanelWidth.Value
                : 0f;
            _horizontalSplitPanel.fixedPaneInitialDimension = width;
            _filterPanel.style.width = width;
        }
    }    
}
