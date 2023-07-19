namespace EtAlii.UniCon.Editor
{
    using System.Collections.Generic;
    using Serilog.Events;
    using UniRx;
    using UnityEngine.UIElements;

    public class FilterMapping
    {
        private readonly Button _button;
        private readonly ReactiveCommand<FilterMapping> _command;
        public KeyValuePair<string, LogEventPropertyValue> Property { get; }

        public FilterMapping(Button button, KeyValuePair<string, LogEventPropertyValue> property, ReactiveCommand<FilterMapping> command)
        {
            _button = button;
            Property = property;
            _command = command;
            _button.clicked += OnButtonClicked;
        }

        private void OnButtonClicked()
        {
            _command.Execute(this);
        }
    }    
}
