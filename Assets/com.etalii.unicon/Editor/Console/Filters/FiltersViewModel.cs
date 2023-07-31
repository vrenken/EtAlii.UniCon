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
        
        public readonly ReactiveCommand<ClickEvent> SaveFilter = new();
        public readonly ReactiveCommand<ClickEvent> CancelFilter = new();

        public void Bind(ExpressionViewModel expressionViewModel, StreamingViewModel streamingViewModel)
        {
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
                    filter = new LogFilter
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
                    
                    // if (UserSettings.instance.ShowExpressionPanel.Value)
                    // {
                    //     expressionViewModel.ToggleExpressionPanel.Execute(new ClickEvent());
                    // }

                    if (!UserSettings.instance.ShowFilterPanel.Value)
                    {
                        ToggleFilterPanel.Execute(new ClickEvent());
                    }
                });

            CancelFilter
                .Subscribe(_ =>
                {
                    expressionViewModel.ExpressionText.Value = string.Empty;
                    
                    // if (UserSettings.instance.ShowExpressionPanel.Value)
                    // {
                    //     expressionViewModel.ToggleExpressionPanel.Execute(new ClickEvent());
                    // }

                    if (!UserSettings.instance.ShowFilterPanel.Value)
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

        private bool NameIsValid(string text, LogFilter logFilter)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            var matchingRule = CustomFilters.SingleOrDefault(r => r.Name == text);
            return matchingRule == null || matchingRule == logFilter;
        }

        private void SaveCustomFilters()
        {
            
        }
    }    
}
