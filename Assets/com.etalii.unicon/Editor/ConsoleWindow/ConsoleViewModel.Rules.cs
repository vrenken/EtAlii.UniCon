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
        public event Action<string> RulesChanged;

        public readonly List<FilterRule> FilterRules = new ();

        public readonly ReactiveCommand<ClickEvent> OnRulesButtonClick = new();

        public readonly ReactiveCommand<FilterMapping> OnAddExcludeFilterClicked = new();
        public readonly ReactiveCommand<FilterMapping> OnAddIncludeFilterClicked = new();

        private void SetupRules()
        {
            OnRulesButtonClick.Subscribe(_ =>
            {
                Settings.ShowRules = !Settings.ShowRules;
                RulesChanged?.Invoke(nameof(Settings.ShowRules));
            });
            
            OnAddIncludeFilterClicked.Subscribe(e =>
            {
                Settings.ShowRules = true;
                RulesChanged?.Invoke(nameof(Settings.ShowRules));

                var rule = new FilterRule(e.Property.Key, e.Property.Value, FilterType.Is);
                FilterRules.Add(rule);
                RulesChanged?.Invoke(nameof(FilterRules));
                ConfigureStream();
            });
            OnAddExcludeFilterClicked.Subscribe(e =>
            {
                Settings.ShowRules = true;
                RulesChanged?.Invoke(nameof(Settings.ShowRules));

                var rule = new FilterRule(e.Property.Key, e.Property.Value, FilterType.IsNot);
                FilterRules.Add(rule);
                RulesChanged?.Invoke(nameof(FilterRules));
                ConfigureStream();
            });
        }
    }    
}
