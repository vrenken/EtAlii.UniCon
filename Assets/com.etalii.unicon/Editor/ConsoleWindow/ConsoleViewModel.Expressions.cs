namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleViewModel
    {
        /// <summary>
        /// Gets raised when any property related to the rules has changed and the view needs to update itself accordingly.
        /// </summary>
        public event Action<string> ExpressionChanged;

        public readonly List<FilterRule> SelectedFilterRules = new ();

        public readonly ReactiveCommand<ClickEvent> OnExpressionButtonClick = new();

        public readonly ReactiveCommand<FilterMapping> OnAddExcludeFilterClicked = new();
        public readonly ReactiveCommand<FilterMapping> OnAddIncludeFilterClicked = new();

        public readonly ReactiveProperty<string> ExpressionText = new();
        
        public readonly FilterRule ActiveFilterRule = new();
        
        private void SetupExpression()
        {
            OnExpressionButtonClick.Subscribe(_ => Settings.ShowExpressionPanel.Value = !Settings.ShowExpressionPanel.Value);

            ExpressionText.Subscribe(s =>
            {
                ActiveFilterRule.Expression = s;
                ExpressionChanged?.Invoke(nameof(ActiveFilterRule));
                ConfigureStream();
            });
            
            OnAddIncludeFilterClicked.Subscribe(e =>
            {
                Settings.ShowExpressionPanel.Value = true;

                var expression = $"{e.Property.Key} = '{e.Property.Value.ToString().Trim('"')}'";
                ActiveFilterRule.Expression = string.IsNullOrWhiteSpace(ActiveFilterRule.Expression) 
                    ? $"{expression}" 
                    : $"{ActiveFilterRule.Expression.TrimEnd()} and {expression}";
                ExpressionChanged?.Invoke(nameof(ActiveFilterRule));
                ConfigureStream();
            });
            OnAddExcludeFilterClicked.Subscribe(e =>
            {
                Settings.ShowExpressionPanel.Value = true;

                var expression = $"{e.Property.Key} != '{e.Property.Value.ToString().Trim('"')}'";
                ActiveFilterRule.Expression = string.IsNullOrWhiteSpace(ActiveFilterRule.Expression) 
                    ? $"{expression}" 
                    : $"{ActiveFilterRule.Expression.TrimEnd()} and {expression}";
                ExpressionChanged?.Invoke(nameof(ActiveFilterRule));
                ConfigureStream();
            });
        }
    }    
}
