namespace EtAlii.UniCon.Editor
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class ConsoleWindow : EditorWindow, IHasCustomMenu
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
#if UNICON_LIFETIME_DEBUG            
            Debug.Log($"[UNICON] {GetType().Name}.{nameof(OnEnable)}()");
#endif
            titleContent = new GUIContent("UniCon Console", logo.texture);
        }

        private void OnDestroy()
        {
            _view?.Unbind();
            _view = null;
        }

        public void CreateGUI()
        {
#if UNICON_LIFETIME_DEBUG            
            Debug.Log($"[UNICON] {GetType().Name}.{nameof(CreateGUI)}()");
#endif            
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
#if UNICON_LIFETIME_DEBUG            
            Debug.Log($"[UNICON] {GetType().Name}.{nameof(OnDidOpenScene)}()");
#endif
            _view.StretchToParentSize();
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            var settingsMenuButton = new GUIContent("Settings");
            menu.AddItem(settingsMenuButton, false, OnShowSettingsMenu);           
        }

        private void OnShowSettingsMenu()
        {
            GetWindow<SettingsWindow>();
        }
    }    
}
