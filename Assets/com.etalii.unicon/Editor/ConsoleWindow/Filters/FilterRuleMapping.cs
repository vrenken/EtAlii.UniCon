namespace EtAlii.UniCon.Editor
{
    using UniRx;
    using UnityEngine.UIElements;

    public class FilterRuleMapping
    {
        private readonly Button _button;
        private readonly ReactiveCommand<FilterRuleMapping> _command;
        public FilterRule FilterRule { get; }

        public FilterRuleMapping(Button button, FilterRule filterRule, ReactiveCommand<FilterRuleMapping> command)
        {
            _button = button;
            FilterRule = filterRule;
            _command = command;
            _button.clicked += OnButtonClicked;
        }

        private void OnButtonClicked()
        {
            _command.Execute(this);
        }
    }    
}
