namespace EtAlii.UniCon.Editor
{
    using RedMoon.ReactiveKit;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleView
    {
        private readonly Button _rulesButton;
        private readonly VisualElement _rulesPanel;

        private void BindRules(ConsoleViewModel viewModel, CompositeDisposable disposable)
        {
            UpdateToggleButton(_rulesButton, _viewModel.Settings.ShowRules);
            UpdateRulesPanel();

            _filterButton
                .BindClick(viewModel.OnFilterButtonClick)
                .AddTo(disposable);

            _rulesButton
                .BindClick(viewModel.OnRulesButtonClick)
                .AddTo(disposable);
        }
        
        private void OnRulesChanged(string settingName)
        {
            switch (settingName)
            {
                case nameof(_viewModel.Settings.ShowRules):
                    UpdateToggleButton(_rulesButton, _viewModel.Settings.ShowRules);
                    UpdateRulesPanel();
                    break;
            }
        }

        private void UpdateRulesPanel()
        {
            _rulesPanel.visible = _viewModel.Settings.ShowRules; 
            //_rulesPanel.style.display = _rulesPanel.visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }    
}