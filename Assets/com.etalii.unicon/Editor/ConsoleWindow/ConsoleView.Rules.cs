namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Linq;
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
            foreach (var filterRule in _viewModel.FilterRules.OrderBy(r => r.Property))
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

                var propertyLabel = new Label
                {
                    text = filterRule.Property,
                    style =
                    {
                        marginLeft = 5,
                        flexGrow = 0, 
                        width = 200, 
                        unityTextAlign = TextAnchor.MiddleLeft
                    }
                };
                filterRuleView.contentContainer.Add(propertyLabel);

                var dropDown = new DropdownField
                {
                    value = filterRule.FilterType.ToString(),
                    choices = Enum.GetNames(typeof(FilterType)).ToList(),
                    style =
                    {
                        width = 100
                    }
                };
                filterRuleView.contentContainer.Add(dropDown);

                var valueTextField = new TextField
                {
                    multiline = false,
                    value = filterRule.Value.ToString().Trim('"'),
                    style =
                    {
                        flexGrow = 1
                    }
                };
                filterRuleView.contentContainer.Add(valueTextField);
                
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
            _rulesPanel.visible = _viewModel.Settings.ShowRules; 
            //_rulesPanel.style.display = _rulesPanel.visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }    
}