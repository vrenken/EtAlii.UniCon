namespace EtAlii.UniCon.Editor
{
    using System.Linq;
    using UniRx;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class FiltersViewModel
    {
        public readonly ReactiveCollection<LogFilter> CustomFilters = new();

        public readonly ReactiveCommand<ClickEvent> ToggleFilterPanel = new();
        
        public readonly ReactiveCommand<ClickEvent> SaveFilter = new();
        public readonly ReactiveCommand<ClickEvent> CancelFilter = new();
        private StreamingViewModel _streamingViewModel;

        public void Bind(ExpressionViewModel expressionViewModel, StreamingViewModel streamingViewModel)
        {
            _streamingViewModel = streamingViewModel;
            ToggleFilterPanel
                .Subscribe(_ =>
                {
                    UserSettings.instance.ShowFilterPanel.Value = !UserSettings.instance.ShowFilterPanel.Value;
                    UserSettings.instance.FilterPanelWidth.Value = UserSettings.instance.FilterPanelWidth.Value;
                });

            UserSettings.instance.UseSerilogSource.Subscribe(_ => streamingViewModel.ConfigureStream());
            UserSettings.instance.UseUnitySource.Subscribe(_ => streamingViewModel.ConfigureStream());
            UserSettings.instance.LogLevel.Subscribe(_ => streamingViewModel.ConfigureStream());
            UserSettings.instance.ShowExceptions.Subscribe(_ => streamingViewModel.ConfigureStream());

            SaveFilter
                .Subscribe(_ =>
                {
                    var filter = CustomFilters.SingleOrDefault(f => f.IsEditing.Value);
                    var filterToMatch = filter;
                    var filterName = EditorInputDialog
                        .Show(
                            "Filter name", 
                            string.Empty, 
                            (filter != null ? filter.Name.Value : null) ?? "New filter",
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
                        filter = ScriptableObject.CreateInstance<LogFilter>();
                        filter.Bind();
                        filter.IsActive.Subscribe(_ => OnCustomFiltersChanged());
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

            CancelFilter
                .Subscribe(_ =>
                {
                    expressionViewModel.ExpressionText.Value = string.Empty;

                    if (!UserSettings.instance.ShowFilterPanel.Value)
                    {
                        ToggleFilterPanel.Execute(new ClickEvent());
                    }
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
            _streamingViewModel.ConfigureStream();
        }
    }    
}
