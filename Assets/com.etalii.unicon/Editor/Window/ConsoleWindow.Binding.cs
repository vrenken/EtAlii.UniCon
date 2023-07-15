namespace EtAlii.UniCon.Editor
{
    using UnityEngine.UIElements;
    using Serilog.Formatting;
    using Serilog.Formatting.Display;
    using System.IO;
    
    public partial class ConsoleWindow
    {
        private const string DefaultDebugOutputTemplate = "[{Level:u3}] {Message:lj}{NewLine}{Exception}";

        private readonly ITextFormatter _formatter = new MessageTemplateTextFormatter(DefaultDebugOutputTemplate);

        private void RemoveEntry(VisualElement element)
        {
        }

        private VisualElement AddLogEntry()
        {
            return new Foldout();
        }

        private void BindLogEntry(VisualElement element, int index)
        {
            var logEvent = LogSink.LogEvents[index];
            
            using var buffer = new StringWriter();
            _formatter.Format(logEvent, buffer);
            var message = buffer.ToString().Trim();

            element.userData = logEvent;
            ((Foldout)element).text = message;
        }

        private void UnbindEntry(VisualElement element, int index)
        {
            element.userData = null;
        }

    }    
}
