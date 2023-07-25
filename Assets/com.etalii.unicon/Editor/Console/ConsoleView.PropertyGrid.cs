namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Serilog.Events;
    using Serilog.Expressions.Compilation.Linq;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Cursor = UnityEngine.UIElements.Cursor;

    public partial class ConsoleView
    {
        private readonly Color _propertyGridActionColor;
        
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

            var eventIdHash = EventIdHash.Compute(logEvent.MessageTemplate.Text);
                
            var headerRow = new VisualElement
            {
                name = $"header-row",
                style =
                {
                    alignContent = Align.Stretch, 
                    alignItems = Align.FlexStart,
                    flexGrow = 1, 
                    flexDirection = FlexDirection.Row
                }
            };
            
            var eventDropDownButton = new Button
            {
                name = $"{eventIdHash:X8}-event-dropdown",
                text = "Event ∨",
                focusable = false,
                style =
                {
                    cursor = GetCursor(MouseCursor.ArrowPlus),
                    //unityFontStyleAndWeight = FontStyle.Bold,
                    color = _propertyGridActionColor,
                    borderBottomWidth = 0, borderLeftWidth = 0, borderTopWidth = 0, borderRightWidth = 0,
                    backgroundColor = Color.clear,
                }
            };
            RegisterContextMenu(eventDropDownButton, evt =>
            {
                evt.menu.AppendAction("Find just this", _ => { }, DropdownMenuAction.Status.Disabled);
                evt.menu.AppendAction("Find with predecessors", _ => { }, DropdownMenuAction.Status.Disabled);
                evt.menu.AppendAction("Find with adjacent", _ => { }, DropdownMenuAction.Status.Disabled);
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Seek to ±5 seconds", _ => _viewModel.AddSeekToTimeSpanToExpression.Execute((logEvent.Timestamp, TimeSpan.FromSeconds(5))));
                evt.menu.AppendAction("Seek to ±30 seconds", _ => _viewModel.AddSeekToTimeSpanToExpression.Execute((logEvent.Timestamp, TimeSpan.FromSeconds(30))));
                evt.menu.AppendAction("Seek to ±5 minutes", _ => _viewModel.AddSeekToTimeSpanToExpression.Execute((logEvent.Timestamp, TimeSpan.FromMinutes(5))));
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Search FROM timestamp", _ => { }, DropdownMenuAction.Status.Disabled);
                evt.menu.AppendAction("Search TO timestamp", _ => { }, DropdownMenuAction.Status.Disabled);
            });
            headerRow.contentContainer.Add(eventDropDownButton);

            var levelDropDownButton = new Button
            {
                name = $"{eventIdHash:X8}-level-dropdown",
                text = "Level (Information) ∨",
                focusable = false,
                style =
                {
                    cursor = GetCursor(MouseCursor.ArrowPlus),
                    //unityFontStyleAndWeight = FontStyle.Bold,
                    color = _propertyGridActionColor,
                    borderBottomWidth = 0, borderLeftWidth = 0, borderTopWidth = 0, borderRightWidth = 0,
                    backgroundColor = Color.clear,
                }
            };
            RegisterContextMenu(levelDropDownButton, evt =>
            {
                evt.menu.AppendAction("Find", _ => _viewModel.AddFindByLogLevelToExpression.Execute(logEvent.Level));
                evt.menu.AppendAction("Exclude", _ => _viewModel.AddExcludeByLogLevelToExpression.Execute(logEvent.Level));
                evt.menu.AppendAction("Set as minimum level", _ => { }, DropdownMenuAction.Status.Disabled);
            });
            headerRow.contentContainer.Add(levelDropDownButton);

            var typeDropDownButton = new Button
            {
                name = $"{eventIdHash:X8}-type-dropdown",
                text = $"Type (0x{eventIdHash:X8}) ∨",
                focusable = false,
                style =
                {
                    cursor = GetCursor(MouseCursor.ArrowPlus),
                    //unityFontStyleAndWeight = FontStyle.Bold,
                    color = _propertyGridActionColor,
                    borderBottomWidth = 0, borderLeftWidth = 0, borderTopWidth = 0, borderRightWidth = 0,
                    backgroundColor = Color.clear,
                }
            };
            RegisterContextMenu(typeDropDownButton, evt =>
            {
                evt.menu.AppendAction("Find", _ => _viewModel.AddFindByEventTypeToExpression.Execute(eventIdHash));
                evt.menu.AppendAction("Find from template", _ => { }, DropdownMenuAction.Status.Disabled);
                evt.menu.AppendAction("Exclude", _ => _viewModel.AddExcludeByEventTypeToExpression.Execute(eventIdHash));
            });
            headerRow.contentContainer.Add(typeDropDownButton);

            grid.contentContainer.Add(headerRow); 

            
            //foreach (var property in logEvent.Properties.OrderBy(p => p.Key))
            foreach (var property in logEvent.Properties.OrderBy(p => p.Key))
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

                var addIncludeToFilterButton = new Button
                {
                    text = $"v", // \u2611 \u2713 \u002B
                    focusable = false,
                    style =
                    {
                        cursor = GetCursor(MouseCursor.ArrowPlus),
                        fontSize = new StyleLength(new Length(15, LengthUnit.Pixel)),
                        color = _propertyGridActionColor,
                        marginLeft = 4, marginRight = 6, marginTop = -4,
                        width = 14, maxWidth = 14,
                        backgroundColor = Color.clear,
                        borderBottomWidth = 0,
                        borderLeftWidth = 0,
                        borderTopWidth = 0,
                        borderRightWidth = 0,
                        unityTextAlign = TextAnchor.MiddleLeft
                    }
                };
                RegisterContextMenu(addIncludeToFilterButton, evt =>
                {
                    evt.menu.AppendAction("Find", _ => _viewModel.AddFindByPropertyToExpression.Execute(property));
                    evt.menu.AppendAction("Find on this event type", _ => { }, DropdownMenuAction.Status.Disabled);
                    evt.menu.AppendAction("Find with any value", _ => _viewModel.AddFindWithAnyPropertyValueToExpression.Execute(property.Key));
                });
                row.contentContainer.Add(addIncludeToFilterButton);

                var addExcludeToFilterButton = new Button
                {
                    text = $"x", // \u2612 \u00D7
                    focusable = false,
                    style =
                    {
                        cursor = GetCursor(MouseCursor.ArrowPlus),
                        fontSize = new StyleLength(new Length(15, LengthUnit.Pixel)),
                        color = _propertyGridActionColor,
                        marginLeft = 0, marginRight = 10, marginTop = -4,
                        width = 14, maxWidth = 14,
                        backgroundColor = Color.clear,
                        borderBottomWidth = 0,
                        borderLeftWidth = 0,
                        borderTopWidth = 0,
                        borderRightWidth = 0,
                        unityTextAlign = TextAnchor.MiddleLeft
                    }
                };
                RegisterContextMenu(addExcludeToFilterButton, evt =>
                {
                    evt.menu.AppendAction("Exclude", _ => _viewModel.AddExcludeByPropertyToExpression.Execute(property));
                    evt.menu.AppendAction("Exclude on this event type", _ => { }, DropdownMenuAction.Status.Disabled);
                    evt.menu.AppendAction("Exclude with any value", _ => _viewModel.AddExcludeWithAnyPropertyValueToExpression.Execute(property.Key));
                });
                row.contentContainer.Add(addExcludeToFilterButton);                
                
                var keyLabel = new Label
                {
                    name = $"key-{property.Key}",
                    text = property.Key,
                    style =
                    {
                        flexGrow = 0, 
                        width = 200, 
                        unityTextAlign = TextAnchor.MiddleLeft
                    }
                };
                row.contentContainer.Add(keyLabel);

                row.contentContainer.Add(new Label
                {
                    name = $"value-{property.Key}", 
                    text = property.Value.ToString().Trim('"'), 
                    style =
                    {
                        flexGrow = 1, 
                        alignSelf = Align.FlexStart, 
                        unityTextAlign = TextAnchor.MiddleLeft
                    }
                });

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

        /// <summary>
        /// Helper method to register a (left-mouse enabled) context menu for the specified visual element.
        /// Please note that the ElementAwareContextualMenuManipulator does some nifty layout magic to ensure
        /// that the context menu shows up below the visual element.
        /// </summary>
        /// <param name="visualElement"></param>
        /// <param name="evt"></param>
        private void RegisterContextMenu(VisualElement visualElement, Action<ContextualMenuPopulateEvent> evt)
        {
            var manipulator = new ElementAwareContextualMenuManipulator(evt)
            {
                target = visualElement
            };
            manipulator.activators.Clear();
            manipulator.activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
            visualElement.AddManipulator(manipulator);
        }
        
        /// <summary>
        /// A helper method to quickly assign a cursor to a visual element when it is hovered over by the mouse.  
        /// </summary>
        /// <param name="cursor"></param>
        /// <returns></returns>
        private StyleCursor GetCursor(MouseCursor cursor)
        {
            object cursorInstance = new Cursor();
            var fields = typeof(Cursor).GetProperty("defaultCursorId", BindingFlags.NonPublic | BindingFlags.Instance)!;
            fields.SetValue(cursorInstance, (int)cursor);
            return new StyleCursor((Cursor)cursorInstance);
        }
    }    
}
