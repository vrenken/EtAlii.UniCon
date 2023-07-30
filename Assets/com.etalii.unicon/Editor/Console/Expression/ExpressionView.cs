﻿namespace EtAlii.UniCon.Editor
{
    using System;
    using RedMoon.ReactiveKit;
    using UniRx;
    using UnityEngine.UIElements;

    public class ExpressionView
    {
        private readonly TwoPaneSplitView _verticalSplitPanel;

        private readonly Button _expressionButton;
        private readonly VisualElement _expressionPanel;
        private readonly TextField _expressionTextField;
        private readonly Button _expressionErrorButton;
        private readonly Button _expressionSaveButton;
        private readonly Button _expressionCancelButton;
        private ExpressionViewModel _viewModel;


        public ExpressionView(VisualElement root)
        {
            _verticalSplitPanel = root.Q<TwoPaneSplitView>("vertical-split-panel");
            _expressionPanel = root.Q<VisualElement>("expression-panel");
            _expressionButton = root.Q<Button>("expression-button");
            _expressionTextField = root.Q<TextField>("expression-textfield");
            _expressionErrorButton = root.Q<Button>("expression-error-button");
            _expressionSaveButton = root.Q<Button>("expression-save-button");
            _expressionCancelButton = root.Q<Button>("expression-cancel-button");
        }

        public void Bind(ExpressionViewModel viewModel, FiltersViewModel filtersViewModel, CompositeDisposable disposable)
        {
            _viewModel = viewModel;
            
            _expressionButton
                .BindClick(viewModel.ToggleExpressionPanel)
                .AddTo(disposable);
            viewModel.UserSettings.ShowExpressionPanel
                .Subscribe(showExpressionPanel =>
                {
                    UpdateExpressionPanel();
                    _expressionButton.UpdateToggleButton(showExpressionPanel);
                })
                .AddTo(disposable);
            _expressionPanel
                .BindCallback<GeometryChangedEvent>(_ =>
                {
                    if (_expressionPanel.visible && _expressionPanel.contentRect.height > 0)
                    {
                        _viewModel.UserSettings.ExpressionPanelHeight.Value = _expressionPanel.contentRect.height;
                    }
                })
                .AddTo(disposable);
            _viewModel.UserSettings.ExpressionPanelHeight
                .Throttle(TimeSpan.FromMilliseconds(300))
                .Subscribe(_ => UpdateExpressionPanel())
                .AddTo(disposable);
            
            _expressionTextField
                .BindTwoWayValueChanged(filtersViewModel.SelectedCustomFilter.Expression)
                .AddTo(disposable);

            filtersViewModel.SelectedCustomFilter.CompiledExpression
                .Subscribe(_ =>
                {
                    _expressionTextField.value = filtersViewModel.SelectedCustomFilter.Expression.Value;
                    var compiledExpression = filtersViewModel.SelectedCustomFilter.CompiledExpression.Value;
                    _expressionErrorButton.text = compiledExpression != null ? "Ok" : filtersViewModel.SelectedCustomFilter.Error;
                    _expressionSaveButton.SetEnabled(compiledExpression != null);
                })
                .AddTo(disposable);
            
            _expressionSaveButton
                .BindClick(filtersViewModel.SaveFilter)
                .AddTo(disposable);

            _expressionCancelButton
                .BindClick(filtersViewModel.CancelFilter)
                .AddTo(disposable);
        }

        public void Unbind()
        {
        }

        private void UpdateExpressionPanel()
        {
            _expressionPanel.visible = _viewModel.UserSettings.ShowExpressionPanel.Value; 
            
            var height = _expressionPanel.visible
                ? _viewModel.UserSettings.ExpressionPanelHeight.Value
                : 0f;
            _verticalSplitPanel.fixedPaneInitialDimension = height;
            _expressionPanel.style.height = height;
        }
    }
}