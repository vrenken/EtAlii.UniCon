namespace EtAlii.UniCon.Editor
{
    using RedMoon.ReactiveKit;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleView
    {
        private readonly Button _filterButton;
        private readonly Button _rulesButton;

        private void BindRules(ConsoleViewModel viewModel, CompositeDisposable disposable)
        {
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
                    // Configure rules panel
                    break;
            }
        }

    }    
}
