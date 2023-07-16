namespace EtAlii.UniCon.Editor
{
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

        public void Bind(LogEventLineViewModel viewModel)
        {
            _timestampLabel.text = viewModel.timestamp;
            _messageLabel.text = BuildMessage(viewModel);
        }
        
        private string BuildMessage(LogEventLineViewModel viewModel)
        {
            return MarkerMessageTemplateRenderer.Render(viewModel.LogEvent.MessageTemplate, viewModel.LogEvent.Properties);
        }

    }    
}
