namespace EtAlii.UniCon.Editor
{
    using RedMoon.ReactiveKit;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UIElements;

    public partial class ConsoleView
    {
        private readonly Button _rulesButton;
        private readonly VisualElement _rulesPanel;
        private readonly ScrollView _rulesList;

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
            
            _rulesAddButtonMenu.menu.AppendAction("And", _ => { }, _ => DropdownMenuAction.Status.Normal);
            _rulesAddButtonMenu.menu.AppendAction("Or", _ => { }, _ => DropdownMenuAction.Status.Normal);
            _rulesAddButtonMenu.menu.AppendAction("Search", _ => { }, _ => DropdownMenuAction.Status.Normal);
            _rulesAddButtonMenu.menu.AppendAction("Has", _ => { }, _ => DropdownMenuAction.Status.Normal);
            _rulesAddButtonMenu.menu.AppendAction("Does not have", _ => { }, _ => DropdownMenuAction.Status.Normal);
        }
        
        private void OnRulesChanged(string settingName)
        {
            switch (settingName)
            {
                case nameof(_viewModel.Settings.ShowRules):
                    UpdateToggleButton(_rulesButton, _viewModel.Settings.ShowRules);
                    UpdateRulesPanel();
                    break;
                case nameof(_viewModel.FilterRules):
                    UpdatesRulesList();
                    break;
            }
        }

        private void UpdatesRulesList()
        {
            _rulesList.Clear();
            foreach (var filterRule in _viewModel.FilterRules)
            {
                var filterRuleView = new VisualElement
                {
                    style = 
                    {
                        flexGrow = 1, 
                        flexDirection = FlexDirection.Row,
                        width = new StyleLength(new Length(100, LengthUnit.Percent)),
                        height = new StyleLength(StyleKeyword.Auto)
                    }
                };

                var expressionLabel = new TextField
                {
                    value = filterRule.Expression,
                    multiline = true,
                    style =
                    {
                        width = new StyleLength(new Length(100, LengthUnit.Percent)),
                        height = new StyleLength(new Length(100, LengthUnit.Percent))
                    }
                };
                filterRuleView.contentContainer.Add(expressionLabel);
                
                var removeButton = new Button
                {
                    text = $"<b><color=red>\u00D7</color></b>",
                    focusable = false,
                    style =
                    {
                        marginLeft = 0, marginRight = 0,
                        width = 16, maxWidth = 16,
                        backgroundColor = Color.clear,
                        borderBottomWidth = 0,
                        borderLeftWidth = 0,
                        borderTopWidth = 0,
                        borderRightWidth = 0
                    }
                };
                removeButton.userData = new FilterRuleMapping(removeButton, filterRule, _viewModel.OnRemoveFilterRuleButtonClick);
                filterRuleView.contentContainer.Add(removeButton);

                _rulesList.contentContainer.Add(filterRuleView);
            }
        }

        private void UpdateRulesPanel()
        {
            if (_rulesPanel.visible)
            {
                _viewModel.Settings.RulesPanelHeight = _rulesPanel.contentRect.height > 0f
                    ? _rulesPanel.contentRect.height
                    : 150f;
            }

            _rulesPanel.visible = _viewModel.Settings.ShowRules; 
            var height = _rulesPanel.visible
                ? _viewModel.Settings.RulesPanelHeight
                : 0f;
            _verticalSplitPanel.fixedPaneInitialDimension = height;
            _rulesPanel.style.width = height;
        }
    }    
}