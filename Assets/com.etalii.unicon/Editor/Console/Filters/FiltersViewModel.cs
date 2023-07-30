namespace EtAlii.UniCon.Editor
{
    using System.Linq;
    using UniRx;
    using UnityEditor;
    using UnityEngine.UIElements;

    public class FiltersViewModel
    {
        internal UserSettings UserSettings => UserSettings.instance;
        internal ProjectSettings ProjectSettings => ProjectSettings.instance;

        public readonly ReactiveCollection<CustomFilter> CustomFilters = new();

        public readonly ReactiveCommand<ClickEvent> ToggleFilterPanel = new();
        
        public readonly ReactiveCommand<ClickEvent> SaveFilter = new();
        public readonly ReactiveCommand<ClickEvent> CancelFilter = new();

        public void Bind(ExpressionViewModel expressionViewModel, StreamingViewModel streamingViewModel)
        {
            ToggleFilterPanel
                .Subscribe(_ =>
                {
                    UserSettings.ShowFilterPanel.Value = !UserSettings.ShowFilterPanel.Value;
                    UserSettings.FilterPanelWidth.Value = UserSettings.FilterPanelWidth.Value;
                });

            UserSettings.UseSerilogSource.Subscribe(_ => streamingViewModel.ConfigureStream());
            UserSettings.UseUnitySource.Subscribe(_ => streamingViewModel.ConfigureStream());
            UserSettings.LogLevel.Subscribe(_ => streamingViewModel.ConfigureStream());
            UserSettings.ShowExceptions.Subscribe(_ => streamingViewModel.ConfigureStream());

            SaveFilter
                .Subscribe(_ =>
                {
                    var filter = CustomFilters.SingleOrDefault(f => f.IsEditing.Value);
                    var filterToMatch = filter;
                    var filterName = EditorInputDialog
                        .Show(
                            "Filter name", 
                            string.Empty, 
                            filter?.Name ?? "New filter",
                            parentWindow: EditorWindow.GetWindow<ConsoleWindow>(),
                            textValidation: text => NameIsValid(text, filterToMatch));

                    if (filterName == null)
                    {
                        // If the dialog is cancelled no filter name will be specified.
                        // In that case we cancel the whole save.
                        return;
                    }

                    var isNew = filter == null;
                    filter = new CustomFilter
                    {
                        Name = filterName,
                        Expression = { Value = expressionViewModel.ExpressionText.Value },
                        IsActive = { Value = true },
                        IsEditing = { Value = false }
                    };
                    if (isNew)
                    {
                        CustomFilters.Add(filter);
                    }

                    SaveCustomFilters();

                    expressionViewModel.ExpressionText.Value = string.Empty;
                    
                    // if (UserSettings.ShowExpressionPanel.Value)
                    // {
                    //     expressionViewModel.ToggleExpressionPanel.Execute(new ClickEvent());
                    // }

                    if (!UserSettings.ShowFilterPanel.Value)
                    {
                        ToggleFilterPanel.Execute(new ClickEvent());
                    }
                });

            CancelFilter
                .Subscribe(_ =>
                {
                    expressionViewModel.ExpressionText.Value = string.Empty;
                    
                    // if (UserSettings.ShowExpressionPanel.Value)
                    // {
                    //     expressionViewModel.ToggleExpressionPanel.Execute(new ClickEvent());
                    // }

                    if (!UserSettings.ShowFilterPanel.Value)
                    {
                        ToggleFilterPanel.Execute(new ClickEvent());
                    }
                });

            CustomFilters
                .ObserveAdd()
                .Subscribe(evt =>
                {
                    evt.Value.IsActive.Subscribe(_ =>
                    {
                        SaveCustomFilters();
                        streamingViewModel.ConfigureStream();
                    });
                });
        }

        private bool NameIsValid(string text, CustomFilter customFilter)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            var matchingRule = CustomFilters.SingleOrDefault(r => r.Name == text);
            return matchingRule == null || matchingRule == customFilter;
        }

        private void SaveCustomFilters()
        {
            
        }
    }    
}
