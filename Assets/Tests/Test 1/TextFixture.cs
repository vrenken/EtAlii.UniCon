using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tests
{
    public class TextFixture : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset visualTreeAsset;

        public static void ShowExample()
        {
            var wnd = GetWindow<TextFixture>();
            wnd.titleContent = new GUIContent("TextFixture");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;

            // VisualElements objects can contain other VisualElement following a tree hierarchy.
            var label = new Label("Hello World! From C#");
            root.Add(label);

            // Instantiate UXML
            var labelFromUxml = visualTreeAsset.Instantiate();
            root.Add(labelFromUxml);
        }
    }
}
