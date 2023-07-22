namespace EtAlii.UniCon.Editor
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class SettingsWindow : EditorWindow
    {
        private SettingsView _view;
        private SettingsViewModel _viewModel;

        public Sprite logo;
        private void OnEnable()
        {
            titleContent = new GUIContent("UniCon Console - Settings", logo.texture);
        }

        public void CreateGUI()
        {
            // Import UXML
            _view = new SettingsView();
            _viewModel = SettingsViewModel.Instance;
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
