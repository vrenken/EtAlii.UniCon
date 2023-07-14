namespace EtAlii.UniCon
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;


    public class ConsoleWindow : EditorWindow
    {
        [MenuItem("Window/General/Console (UniCon) %#C", false, 7)]
        public static void ShowExample()
        {
            var wnd = GetWindow<ConsoleWindow>();
            wnd.titleContent = new GUIContent("UniCon Console");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;

            // VisualElements objects can contain other VisualElement following a tree hierarchy.
            var label = new Label("Hello World! From C#");
            root.Add(label);

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/com.etalii.unicon/Editor/Window/ConsoleWindow.uxml");
            var labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/com.etalii.unicon/Editor/Window/ConsoleWindow.uss");
            var labelWithStyle = new Label("Hello World! With Style");
            labelWithStyle.styleSheets.Add(styleSheet);
            root.Add(labelWithStyle);
        }
    }    
}
