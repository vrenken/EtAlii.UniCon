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

        public readonly CustomFilter SelectedCustomFilter = new();
        public readonly ReactiveCollection<CustomFilter> CustomFilters = new();

        public readonly ReactiveCommand<ClickEvent> ToggleFilterPanel = new();
        
        public readonly ReactiveCommand<ClickEvent> SaveFilter = new();
        public readonly ReactiveCommand<ClickEvent> CancelFilter = new();

        public void Bind(ExpressionViewModel expressionViewModel, StreamingViewModel streamingViewModel)
        {
            SelectedCustomFilter.Expression
                .Subscribe(_ =>
                {
                    streamingViewModel.ConfigureStream();
                });

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
                    SelectedCustomFilter.Name = EditorInputDialog
                        .Show(
                            "Filter name", 
                            string.Empty, 
                            SelectedCustomFilter.Name ?? "New filter",
                            parentWindow: EditorWindow.GetWindow<ConsoleWindow>(),
                            textValidation: text => NameIsValid(text, SelectedCustomFilter));

                    SelectedCustomFilter.IsActive.Value = true;
                    if (!CustomFilters.Contains(SelectedCustomFilter))
                    {
                        CustomFilters.Add(SelectedCustomFilter);
                    }

                    SelectedCustomFilter.Expression.Value = string.Empty;
                    
                    if (UserSettings.ShowExpressionPanel.Value)
                    {
                        expressionViewModel.ToggleExpressionPanel.Execute(new ClickEvent());
                    }

                    if (UserSettings.ShowFilterPanel.Value == false)
                    {
                        ToggleFilterPanel.Execute(new ClickEvent());
                    }
                });

            CancelFilter
                .Subscribe(_ =>
                {
                    SelectedCustomFilter.Expression.Value = string.Empty;
                    
                    if (UserSettings.ShowExpressionPanel.Value)
                    {
                        expressionViewModel.ToggleExpressionPanel.Execute(new ClickEvent());
                    }

                    if (UserSettings.ShowFilterPanel.Value == false)
                    {
                        ToggleFilterPanel.Execute(new ClickEvent());
                    }
                });

            CustomFilters
                .ObserveAdd()
                .Subscribe(evt => evt.Value.IsActive.Subscribe(_ => streamingViewModel.ConfigureStream()));
        }

        private bool NameIsValid(string text, CustomFilter rule)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            var matchingRule = CustomFilters.SingleOrDefault(r => r.Name == text);
            return matchingRule == null || matchingRule == rule;
        }
    }    
}
