namespace EtAlii.UniCon.Editor
{
    using UniRx;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public partial class ConsoleView : VisualElement
    {
        private readonly ToolbarMenu _expressionAddButtonMenu;

        private ConsoleViewModel _viewModel;
        private CompositeDisposable _disposables;

        private readonly ExpressionView _expression;
        private readonly FiltersView _filters;
        private readonly StreamingView _streaming;
        
        public new class UxmlFactory : UxmlFactory<ConsoleView, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                var visualTree = Resources.Load<VisualTreeAsset>(nameof(ConsoleView));
                var root = base.Create(bag, cc);
                visualTree.CloneTree(root);
                return root;
            }
        }
 
        public new class UxmlTraits : VisualElement.UxmlTraits {}

        public ConsoleView()
        {
            var visualTree = Resources.Load<VisualTreeAsset>(nameof(ConsoleView));
            visualTree.CloneTree(this);

            _expression = new ExpressionView(this);
            _filters = new FiltersView(this);
            _streaming = new StreamingView(this);
            
            var clearButtonMenu = this.Q<ToolbarMenu>("clear-button");
            clearButtonMenu.menu.AppendAction("Clear", OnClearAction, OnClearActionCallBack);
            clearButtonMenu.menu.AppendAction("Clear on Play", OnClearOnPlayAction, OnClearOnPlayActionCallBack);
            clearButtonMenu.menu.AppendAction("Clear on Build", OnClearOnBuildAction, OnClearOnBuildActionCallBack);
            clearButtonMenu.menu.AppendAction("Clear on Recompile", OnClearOnRecompileAction, OnClearOnRecompileActionCallBack);
            
#if UNICON_LIFETIME_DEBUG            
            Debug.Log($"[UNICON] {GetType().Name}.ctor()");
#endif
        }
        
        public void Bind(ConsoleViewModel viewModel)
        {
#if UNICON_LIFETIME_DEBUG            
            Debug.Log($"[UNICON] {GetType().Name}.{nameof(Bind)}()");
#endif
            Unbind();

            _viewModel = viewModel;

            _streaming.Bind(viewModel, viewModel.Expressions, viewModel.DataStreamer, _disposables);
            _filters.Bind(viewModel.Filters , _disposables);
            _expression.Bind(viewModel.Expressions, viewModel.Filters, _disposables);
        }

        public void Unbind()
        {
#if UNICON_LIFETIME_DEBUG            
            Debug.Log($"[UNICON] {GetType().Name}.{nameof(Unbind)}()");
#endif
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();

            if (_viewModel == null) return;
            _expression.Unbind();
            _filters.Unbind();
            _streaming.Unbind();
        }
    }
}
