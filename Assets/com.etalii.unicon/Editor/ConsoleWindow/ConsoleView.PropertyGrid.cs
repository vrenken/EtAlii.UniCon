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
                        alignContent = Align.Stretch, 
                        alignItems = Align.FlexStart,
                        flexGrow = 1, 
                        flexDirection = FlexDirection.Row
                    }
                    
                };
                var keyLabel = new Label
                {
                    name = $"key-{property.Key}",
                    text = property.Key,
                    style = { flexGrow = 0, width = 200, unityTextAlign = TextAnchor.MiddleLeft }
                };

                var addIncludeToFilterButton = new Button
                {
                    text = $"<b><color=green>\u002B</color></b>",
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
                addIncludeToFilterButton.userData = new FilterMapping(addIncludeToFilterButton, property, _viewModel.OnAddIncludeFilterClicked);
                row.contentContainer.Add(addIncludeToFilterButton);

                var addExcludeToFilterButton = new Button
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
                addExcludeToFilterButton.userData = new FilterMapping(addExcludeToFilterButton, property, _viewModel.OnAddExcludeFilterClicked);
                row.contentContainer.Add(addExcludeToFilterButton);                
                row.contentContainer.Add(keyLabel);
                row.contentContainer.Add(new Label { name = $"value-{property.Key}", text = property.Value.ToString(), style = { flexGrow = 1, alignSelf = Align.FlexStart, unityTextAlign = TextAnchor.MiddleLeft }});

                grid.contentContainer.Add(row);
            }

            if (logEvent.Exception != null)
            {
                var exceptionLabel = new TextElement
                {
                    text = $"{logEvent.Exception}\n{logEvent.Exception.StackTrace}",
                    style =
                    {
                        alignContent = Align.Stretch, 
                        alignItems = Align.FlexStart,
                        flexGrow = 1, 
                        flexDirection = FlexDirection.Row,
                        backgroundColor = new Color(1f, 0.067f, 0.067f, 0.267f),
                        color = Color.white,
                        marginRight = 10,
                        marginBottom = 5,
                        paddingBottom = 3,
                        paddingLeft = 3,
                        paddingRight = 3,
                        paddingTop = 3
                    }
                };
                grid.contentContainer.Add(exceptionLabel);
            }

            return grid;
        }
    }    
}
