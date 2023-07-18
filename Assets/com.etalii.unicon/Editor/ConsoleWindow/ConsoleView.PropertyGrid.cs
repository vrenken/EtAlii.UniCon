namespace EtAlii.UniCon.Editor
{
    using Serilog.Events;
    using UnityEngine;
    using UnityEngine.UIElements;

    public partial class ConsoleView
    {
        private VisualElement BuildPropertyGrid(LogEvent logEvent)
        {
            var grid = new VisualElement
            {
                name = "Grid",
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Column
                }
            };

            //foreach (var property in logEvent.Properties.OrderBy(p => p.Key))
            foreach (var property in logEvent.Properties)
            {
                // Let's skip all internal properties.
                if (property.Key.StartsWith(WellKnownProperties.Prefix)) continue;
                
                var row = new VisualElement
                {
                    name = $"row-{property.Key}",
                    style =
                    {
                        alignItems = Align.FlexStart,
                        flexGrow = 1, 
                        flexDirection = FlexDirection.Row
                    }
                    
                };
                row.contentContainer.Add(new Label { name = $"key-{property.Key}", text = $"<b><color=green>\u002B</color> <color=red>\u00D7</color></b> {property.Key}", style = { flexGrow = 0, width = 200, unityTextAlign = TextAnchor.MiddleLeft }});
                row.contentContainer.Add(new Label { name = $"value-{property.Key}", text = property.Value.ToString(), style = { flexGrow = 1, alignSelf = Align.FlexStart, unityTextAlign = TextAnchor.MiddleLeft }});
                grid.contentContainer.Add(row);
            }

            return grid;
        }
    }    
}
