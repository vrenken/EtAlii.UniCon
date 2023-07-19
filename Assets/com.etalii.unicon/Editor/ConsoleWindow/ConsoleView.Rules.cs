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
                        unityTextAlign = TextAnchor.MiddleLeft
                    }
                };
                filterRuleView.contentContainer.Add(propertyLabel);

                var dropDown = new DropdownField
                {
                    value = filterRule.FilterType.ToString(),
                    choices = Enum.GetNames(typeof(FilterType)).ToList(),
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
                        width = 8, maxWidth = 8,
                        backgroundColor = Color.clear,
                        borderBottomWidth = 0,
                        borderLeftWidth = 0,
                        borderTopWidth = 0,
                        borderRightWidth = 0
                    }
                };
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