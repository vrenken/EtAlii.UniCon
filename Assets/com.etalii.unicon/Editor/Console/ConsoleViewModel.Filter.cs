namespace EtAlii.UniCon.Editor
{
    using System.Linq;
    using UniRx;
    using UnityEditor;
    using UnityEngine.UIElements;

    public partial class ConsoleViewModel
    {
        public CustomFilter SelectedCustomFilter = new();
        public readonly ReactiveCollection<CustomFilter> CustomFilters = new();

        public readonly ReactiveCommand<ClickEvent> ToggleFilterPanel = new();
        
        public readonly ReactiveCommand<ClickEvent> SaveFilter = new();
        public readonly ReactiveCommand<ClickEvent> CancelFilter = new();

        private void SetupFilter()
        {
            ToggleFilterPanel
                .Subscribe(_ =>
                {
                    Settings.ShowFilterPanel.Value = !Settings.ShowFilterPanel.Value;
                    Settings.FilterPanelWidth.SetValueAndForceNotify(Settings.FilterPanelWidth.Value);
                });

            Settings.UseSerilogSource.Subscribe(_ => ConfigureStream());
            Settings.UseUnitySource.Subscribe(_ => ConfigureStream());
            Settings.LogLevel.Subscribe(_ => ConfigureStream());
            Settings.ShowExceptions.Subscribe(_ => ConfigureStream());

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

                    SelectedCustomFilter = new CustomFilter();
                    ExpressionText.Value = string.Empty;
                    
                    if (Settings.ShowExpressionPanel.Value)
                    {
                        ToggleExpressionPanel.Execute(new ClickEvent());
                    }

                    if (Settings.ShowFilterPanel.Value == false)
                    {
                        ToggleFilterPanel.Execute(new ClickEvent());
                    }
                });

            CancelFilter
                .Subscribe(_ =>
                {
                    SelectedCustomFilter = new CustomFilter();
                    ExpressionText.Value = string.Empty;
                    
                    if (Settings.ShowExpressionPanel.Value)
                    {
                        ToggleExpressionPanel.Execute(new ClickEvent());
                    }

                    if (Settings.ShowFilterPanel.Value == false)
                    {
                        ToggleFilterPanel.Execute(new ClickEvent());
                    }
                });

            CustomFilters
                .ObserveAdd()
                .Subscribe(evt => evt.Value.IsActive.Subscribe(_ => ConfigureStream()));
        }

        private bool NameIsValid(string text, CustomFilter rule)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            var matchingRule = CustomFilters.SingleOrDefault(r => r.Name == text);
            return matchingRule == null || matchingRule == rule;
        }
    }    
}
