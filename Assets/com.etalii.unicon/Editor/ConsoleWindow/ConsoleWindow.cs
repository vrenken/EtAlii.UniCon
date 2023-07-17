namespace EtAlii.UniCon.Editor
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class ConsoleWindow : EditorWindow
    {
        private ConsoleView _view;
        private ConsoleViewModel _viewModel;

        [MenuItem("Window/General/Console (UniCon) %#C", false, 7)]
        public static void ShowConsole()
        {
            var window = GetWindow<ConsoleWindow>();

            var icon = EditorGUIUtility.IconContent("d_UnityEditor.ConsoleWindow@2x").image;

            window.titleContent = new GUIContent("UniCon Console", icon);
        }
        
        public void CreateGUI()
        {
            // Import UXML
            _view = new ConsoleView();
            _viewModel = ConsoleViewModel.Instance;
            rootVisualElement.Add(_view);
            _view.Bind(_viewModel);
            _view.StretchToParentSize();
        }

        private void OnDidOpenScene()
        {
            _view.StretchToParentSize();
        }

        public void Update() => _viewModel.Update();
    }    
}
