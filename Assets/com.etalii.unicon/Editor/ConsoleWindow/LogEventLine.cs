namespace EtAlii.UniCon.Editor
{
    using System;
    using Serilog.Events;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class LogEventLine : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<LogEventLine, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                var visualTree = Resources.Load<VisualTreeAsset>(nameof(LogEventLine));
                var root = base.Create(bag, cc);
                visualTree.CloneTree(root);
                return root;
            }
        }
 
        public new class UxmlTraits : VisualElement.UxmlTraits {}

        private readonly Label _timestampLabel;
        private readonly Label _messageLabel;

        public LogEventLine()
        {
            var visualTree = Resources.Load<VisualTreeAsset>(nameof(LogEventLine));
            visualTree.CloneTree(this);
            
            _timestampLabel = this.Q<Label>("timestamp");
            _messageLabel = this.Q<Label>("message");
        }

        public void Bind(LogEventViewModel viewModel)
        {
            _timestampLabel.text = viewModel.timestamp;
            
            _messageLabel.text = GetMessage(viewModel);
        }

        public static string GetMessage(LogEventViewModel viewModel)
        {
            var color = viewModel.LogEvent.Level switch
            {
                LogEventLevel.Verbose => "<color=#5A5A5A>",
                LogEventLevel.Information => "<color=white>",
                LogEventLevel.Debug => "<color=#808080>",
                LogEventLevel.Warning => "<color=yellow>",
                LogEventLevel.Error => "<color=red>",
                LogEventLevel.Fatal => "<color=red>",
                _ => throw new ArgumentOutOfRangeException(nameof(viewModel.LogEvent.Level))
            };
            return $"{viewModel.timestamp} {color}\u25CF</color> " + MarkerMessageTemplateRenderer.Render(viewModel.LogEvent.MessageTemplate, viewModel.LogEvent.Properties);
        }
    }    
}
