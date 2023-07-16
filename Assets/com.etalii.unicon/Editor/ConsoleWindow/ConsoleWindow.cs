namespace EtAlii.UniCon.Editor
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class ConsoleWindow : EditorWindow
    {
        private Rect _previousSize;
        private ConsoleView _view;
        private ConsoleViewModel _viewModel;

        [MenuItem("Window/General/Console (UniCon) %#C", false, 7)]
        public static void ShowConsole()
        {
            var window = GetWindow<ConsoleWindow>();

            window.titleContent = new GUIContent("UniCon Console");
            //window.Initialize();
        }
        
        public void CreateGUI()
        {
            // Import UXML
            _view = new ConsoleView();
            _viewModel = ConsoleViewModel.Instance;
            rootVisualElement.Add(_view);
            _view.Bind(_viewModel);
        }
        
        private void OnGUI()
        {
            if (_previousSize != position)
            {
                _previousSize = position;
                _view.StretchToParentSize();
            }
        }

        // private void OnEnable() => _viewModel?.OnEnable();

        public void Update() => _viewModel?.Update();
    }    
}
