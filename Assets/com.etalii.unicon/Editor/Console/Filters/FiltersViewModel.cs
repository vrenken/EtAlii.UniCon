namespace EtAlii.UniCon.Editor
{
    using System.Linq;
    using UniRx;
    using UnityEditor;
    using UnityEngine.UIElements;

    public class FiltersViewModel
    {
        public readonly ReactiveCollection<LogFilter> CustomFilters = new();

        public readonly ReactiveCommand<ClickEvent> ToggleFilterPanel = new();
        
        public readonly ReactiveCommand<ClickEvent> SaveEditFilter = new();
        public readonly ReactiveCommand<ClickEvent> CancelEditFilter = new();
        public readonly ReactiveCommand<LogFilter> RenameFilter = new();
        public readonly ReactiveCommand<LogFilter> DeleteFilter = new();
        public readonly ReactiveCommand<LogFilter> EditFilter = new();
        
        private readonly DataStreamer _dataStreamer;

        public FiltersViewModel(DataStreamer dataStreamer)
        {
            _dataStreamer = dataStreamer;
        }
        
        public void Bind(ExpressionViewModel expressionViewModel)
        {
            ToggleFilterPanel
                .Subscribe(_ =>
                {
                    UserSettings.instance.ShowFilterPanel.Value = !UserSettings.instance.ShowFilterPanel.Value;
                    UserSettings.instance.FilterPanelWidth.Value = UserSettings.instance.FilterPanelWidth.Value;
                });

            UserSettings.instance.UseSerilogSource.Subscribe(_ => _dataStreamer.ConfigureHard());
            UserSettings.instance.UseUnitySource.Subscribe(_ => _dataStreamer.ConfigureHard());
            UserSettings.instance.LogLevel.Subscribe(_ => _dataStreamer.ConfigureHard());
            UserSettings.instance.ShowExceptions.Subscribe(_ => _dataStreamer.ConfigureHard());

            SaveEditFilter
                .Subscribe(_ =>
                {
                    var filter = CustomFilters.SingleOrDefault(f => f.IsEditing.Value);
                    var filterToMatch = filter;
                    var filterName = EditorInputDialog
                        .Show(
                            "Filter name", 
                            string.Empty, 
                            filter?.Name.Value ?? "New filter",
                            parentWindow: EditorWindow.GetWindow<ConsoleWindow>(),
                            textValidation: text => NameIsValid(text, filterToMatch));

                    if (filterName == null)
                    {
                        // If the dialog is cancelled no filter name will be specified.
                        // In that case we cancel the whole save.
                        return;
                    }

                    var isNew = filter == null;
                    if (isNew)
                    {
                        filter = new LogFilter();
                        filter.Bind();
                    }
                    filter.Name.Value = filterName;
                    filter.Expression.Value = expressionViewModel.ExpressionText.Value;
                    filter.IsActive.Value = true;
                    filter.IsEditing.Value = false;
                    if (isNew)
                    {
                        CustomFilters.Add(filter);
                    }
                    OnCustomFiltersChanged();

                    expressionViewModel.ExpressionText.Value = string.Empty;

                    if (!UserSettings.instance.ShowFilterPanel.Value)
                    {
                        ToggleFilterPanel.Execute(new ClickEvent());
                    }
                });

            CancelEditFilter
                .Subscribe(_ =>
                {
                    expressionViewModel.ExpressionText.Value = string.Empty;
                    var filter = CustomFilters.SingleOrDefault(f => f.IsEditing.Value);
                    if (filter != null)
                    {
                        filter.IsEditing.Value = false;
                    
                        if (!UserSettings.instance.ShowFilterPanel.Value)
                        {
                            ToggleFilterPanel.Execute(new ClickEvent());
                        }
                    }
                });

            
            EditFilter
                .Subscribe(filter =>
                {
                    expressionViewModel.ExpressionText.Value = filter.Expression.Value;
                    filter.IsEditing.Value = true;
                });
            
            DeleteFilter
                .Subscribe(filter => CustomFilters.Remove(filter));
            
            RenameFilter
                .Subscribe(filter =>
                {
                    var filterName = EditorInputDialog
                        .Show(
                            "Rename filter", 
                            string.Empty, 
                            filter.Name.Value,
                            parentWindow: EditorWindow.GetWindow<ConsoleWindow>(),
                            textValidation: text => NameIsValid(text, filter));

                    if (filterName == null)
                    {
                        // If the dialog is cancelled no filter name will be specified.
                        // In that case we cancel the whole rename.
                        return;
                    }

                    filter.Name.Value = filterName;
                });
            CustomFilters
                .ObserveRemove()
                .Subscribe(_ => OnFilterRemoved());
            CustomFilters
                .ObserveAdd()
                .Subscribe(evt => OnFilterAdded(evt.Value));

            foreach (var filter in UserSettings.instance.CustomFilters)
            {
                CustomFilters.Add(filter);
                
            }
        }

        private void OnFilterAdded(LogFilter filter)
        {
            filter.IsEditing.Subscribe(OnFilterIsEditingChanged);
            filter.IsActive.Subscribe(_ => OnCustomFiltersChanged());
            OnCustomFiltersChanged();
        }

        private void OnFilterIsEditingChanged(bool isEditing)
        {
            OnCustomFiltersChanged();
        }

        private void OnFilterRemoved()
        {
            OnCustomFiltersChanged();
        }

        private bool NameIsValid(string text, LogFilter logFilter)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            var matchingRule = CustomFilters.SingleOrDefault(r => r.Name.Value == text);
            return matchingRule == null || matchingRule == logFilter;
        }

        private void OnCustomFiltersChanged()
        {
            UserSettings.instance.CustomFilters = CustomFilters.ToArray();
            _dataStreamer.ConfigureHard();
        }
    }    
}
