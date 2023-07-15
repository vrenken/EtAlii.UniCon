namespace EtAlii.UniCon.Editor
{
    using System;
    using Serilog;
    using Serilog.Events;
    using Serilog.Parsing;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    
    public partial class ConsoleWindow : EditorWindow
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<ConsoleWindow>();
        
        [MenuItem("Window/General/Console (UniCon) %#C", false, 7)]
        public static void ShowConsole()
        {
            var window = GetWindow<ConsoleWindow>();
            window.titleContent = new GUIContent("UniCon Console");
        }

        private Rect _previousSize;
        private TemplateContainer _visualTree;

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;

            // Import UXML
            _visualTree = AssetDatabase
                .LoadAssetAtPath<VisualTreeAsset>("Assets/com.etalii.unicon/Editor/Window/ConsoleWindow.uxml")
                .Instantiate();
            root.Add(_visualTree);
            
            var listView = root.Q<ListView>();
            listView.makeItem = AddLogEntry;
            listView.bindItem = BindLogEntry;
            listView.destroyItem = RemoveEntry;
            listView.unbindItem = UnbindEntry;
            listView.itemsSource = LogSink.LogEvents;

            // var splitView = root.Q<TwoPaneSplitView>("TwoPanelSplitView");
            // splitView.fixedPaneInitialDimension = 400;
            
            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/com.etalii.unicon/Editor/Window/ConsoleWindow.uss");
            //var labelWithStyle = new Label("Hello World! With Style");
            //labelWithStyle.styleSheets.Add(styleSheet);
            //root.Add(labelWithStyle);

            var logEvent = new LogEvent(
                DateTimeOffset.Now, 
                LogEventLevel.Information, 
                null, 
                new MessageTemplate("Started UniCon Console", Array.Empty<MessageTemplateToken>()),
                Array.Empty<LogEventProperty>());
            LogSink.LogEvents.Add(logEvent);
            //_logger.Information("Started UniCon Console");
        }
        
        private void OnGUI()
        {
            if (_previousSize != position)
            {
                _previousSize = position;
                _visualTree.StretchToParentSize();
            }
        }
    }    
}
