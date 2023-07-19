namespace EtAlii.UniCon.Editor
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class ConsoleWindow : EditorWindow
    {
        private ConsoleView _view;
        private ConsoleViewModel _viewModel;

        public Sprite logo;
        
        [MenuItem("Window/General/Console (UniCon) %#C", false, 7)]
        public static void ShowConsole()
        {
            GetWindow<ConsoleWindow>();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("UniCon Console", logo.texture);
        }

        public void CreateGUI()
        {
            // Import UXML
            _view = new ConsoleView();
            _viewModel = ConsoleViewModel.Instance;
            _viewModel.Init();
            rootVisualElement.Add(_view);
            _view.Bind(_viewModel);
            _view.StretchToParentSize();
        }

        private void OnDidOpenScene()
        {
            _view.StretchToParentSize();
        }
    }    
}
