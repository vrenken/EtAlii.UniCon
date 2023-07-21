namespace EtAlii.UniCon.Editor
{
    using RedMoon.ReactiveKit;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleView
    {
        // private readonly MessageTemplateParser _messageTemplateParser = new();
        private readonly Button _expressionButton;
        private readonly VisualElement _expressionPanel;
        private readonly TextField _expressionTextField;
        private readonly Button _expressionErrorButton;
        private readonly Button _expressionSaveButton;

        private void BindExpression(ConsoleViewModel viewModel, CompositeDisposable disposable)
        {
            _expressionButton
                .BindClick(viewModel.OnExpressionButtonClick)
                .AddTo(disposable);
            _viewModel.Settings.ShowExpressionPanel
                .Subscribe(onNext: _ =>
                {
                    UpdateExpressionPanel();
                    UpdateToggleButton(_expressionButton, _viewModel.Settings.ShowExpressionPanel.Value);
                })
                .AddTo(disposable);
            
            _expressionTextField
                //.BindTwoWayValueChanged(viewModel.ExpressionText) // TODO: Try to apply two-way binding here. 
                .BindValueChanged(viewModel.ExpressionText)
                .AddTo(disposable);
        }
        
        private void OnExpressionChanged(string settingName)
        {
            switch (settingName)
            {
                case nameof(_viewModel.ActiveFilterRule):
                    UpdateActiveExpression(_viewModel.ActiveFilterRule);
                    break;
            }
        }

        private void UpdateActiveExpression(FilterRule filterRule)
        {
            // Serilog.Expressions.Compilation.Linq.EventIdHash.Compute(messageTemplate)
            // Serilog.Expressions.Compilation.Linq.EventIdHash.Compute
            // if(ExpressionTemplate.TryParse(
            //     filterRule.Expression,
            //     formatProvider: null,
            //     nameResolver: null,
            //     theme: TemplateTheme.Code,
            //     applyThemeWhenOutputIsRedirected: true,
            //     out var result,
            //     out var error))
            // {
            //     using var sw = new StringWriter();
            //
            //
            //     var messageTemplate = _messageTemplateParser.Parse(filterRule.Expression);
            //     var logEvent = new LogEvent( DateTimeOffset.MinValue, LogEventLevel.Information, null, messageTemplate, Array.Empty<LogEventProperty>());
            //     result.Format(logEvent, sw);
            //     _expressionTextField.value = sw.ToString();
            // }
            // else
            // {
                _expressionTextField.value = filterRule.Expression;
            // }
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

            _expressionPanel.visible = _viewModel.Settings.ShowExpressionPanel.Value; 
            var height = _expressionPanel.visible
                ? _viewModel.Settings.ExpressionPanelHeight
                : 0f;
            _verticalSplitPanel.fixedPaneInitialDimension = height;
            _expressionPanel.style.width = height;
        }
    }    
}