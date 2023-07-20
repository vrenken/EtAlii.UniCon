namespace EtAlii.UniCon.Editor
{
    using RedMoon.ReactiveKit;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleView
    {
        private readonly Button _expressionButton;
        private readonly VisualElement _expressionPanel;
        private readonly TextField _expressionTextField;
        private readonly Button _expressionErrorButton;
        private readonly Button _expressionSaveButton;

        private void BindExpression(ConsoleViewModel viewModel, CompositeDisposable disposable)
        {
            UpdateToggleButton(_expressionButton, _viewModel.Settings.ShowExpressionPanel);
            UpdateExpressionPanel();

            _filterButton
                .BindClick(viewModel.OnFilterButtonClick)
                .AddTo(disposable);

            _expressionButton
                .BindClick(viewModel.OnExpressionButtonClick)
                .AddTo(disposable);

            _expressionTextField
                .BindValueChanged(viewModel.ExpressionText)
                .AddTo(disposable);
        }
        
        private void OnExpressionChanged(string settingName)
        {
            switch (settingName)
            {
                case nameof(_viewModel.Settings.ShowExpressionPanel):
                    UpdateToggleButton(_expressionButton, _viewModel.Settings.ShowExpressionPanel);
                    UpdateExpressionPanel();
                    break;
                case nameof(_viewModel.ActiveFilterRule):
                    UpdateActiveExpression(_viewModel.ActiveFilterRule);
                    break;
            }
        }

        private void UpdateActiveExpression(FilterRule filterRule)
        {
            _expressionTextField.value = filterRule.Expression;
            _expressionErrorButton.text = filterRule.CompiledExpression != null ? "Ok" : filterRule.Error;
            _expressionSaveButton.SetEnabled(filterRule.CompiledExpression != null);
        }

        private void UpdateExpressionPanel()
        {
            if (_expressionPanel.visible)
            {
                _viewModel.Settings.ExpressionPanelHeight = _expressionPanel.contentRect.height > 0f
                    ? _expressionPanel.contentRect.height
                    : 150f;
            }

            _expressionPanel.visible = _viewModel.Settings.ShowExpressionPanel; 
            var height = _expressionPanel.visible
                ? _viewModel.Settings.ExpressionPanelHeight
                : 0f;
            _verticalSplitPanel.fixedPaneInitialDimension = height;
            _expressionPanel.style.width = height;
        }
    }    
}