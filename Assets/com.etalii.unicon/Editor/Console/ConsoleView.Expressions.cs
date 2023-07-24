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
        private readonly Button _expressionCancelButton;

        private void BindExpression(ConsoleViewModel viewModel, CompositeDisposable disposable)
        {
            _expressionButton
                .BindClick(viewModel.ToggleExpressionPanel)
                .AddTo(disposable);
            _viewModel.Settings.ShowExpressionPanel
                .Subscribe(onNext: showExpressionPanel =>
                {
                    UpdateExpressionPanel();
                    UpdateToggleButton(_expressionButton, showExpressionPanel);
                })
                .AddTo(disposable);
            _viewModel.Settings.ExpressionPanelHeight
                .Subscribe(expressionPanelHeight =>
                {
                    var height = _expressionPanel.visible
                        ? expressionPanelHeight
                        : 0f;
                    _verticalSplitPanel.fixedPaneInitialDimension = height;
                    _expressionPanel.style.width = height;
                })
                .AddTo(disposable);
            
            _expressionTextField
                //.BindTwoWayValueChanged(viewModel.ExpressionText) // TODO: Try to apply two-way binding here. 
                .BindValueChanged(viewModel.ExpressionText)
                .AddTo(disposable);

            _expressionSaveButton
                .BindClick(viewModel.SaveFilter)
                .AddTo(disposable);

            _expressionCancelButton
                .BindClick(viewModel.CancelFilter)
                .AddTo(disposable);

        }
        
        private void OnExpressionChanged(string settingName)
        {
            switch (settingName)
            {
                case nameof(_viewModel.SelectedCustomFilter):
                    UpdateActiveExpression(_viewModel.SelectedCustomFilter);
                    break;
            }
        }

        private void UpdateActiveExpression(CustomFilter customFilter)
        {
            _expressionTextField.value = customFilter.Expression;
            _expressionErrorButton.text = customFilter.CompiledExpression != null ? "Ok" : customFilter.Error;
            _expressionSaveButton.SetEnabled(customFilter.CompiledExpression != null);
        }

        private void UpdateExpressionPanel()
        {
            if (_expressionPanel.visible)
            {
                _viewModel.Settings.ExpressionPanelHeight.Value = _expressionPanel.contentRect.height > 0f
                    ? _expressionPanel.contentRect.height
                    : 150f;
            }

            _expressionPanel.visible = _viewModel.Settings.ShowExpressionPanel.Value; 
        }
    }    
}