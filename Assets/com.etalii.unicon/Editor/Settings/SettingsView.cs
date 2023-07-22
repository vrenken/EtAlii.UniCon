namespace EtAlii.UniCon.Editor
{
    using UniRx;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class SettingsView : VisualElement
    {
        private Sprite _logo;
        private SettingsViewModel _viewModel;
        private CompositeDisposable _disposables;

        public new class UxmlFactory : UxmlFactory<ConsoleView, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                var visualTree = Resources.Load<VisualTreeAsset>(nameof(SettingsView));
                var root = base.Create(bag, cc);
                visualTree.CloneTree(root);
                return root;
            }
        }
 
        public new class UxmlTraits : VisualElement.UxmlTraits {}

        public SettingsView()
        {
            var visualTree = Resources.Load<VisualTreeAsset>(nameof(SettingsView));
            visualTree.CloneTree(this);
        }

        
        public void Bind(SettingsViewModel viewModel)
        {
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
            
            if (_viewModel != null)
            {
                // _viewModel.StreamChanged -= OnStreamChanged;
                // _viewModel.ExpressionChanged -= OnExpressionChanged;
            }
            _viewModel = viewModel;

        }
    }    
}
