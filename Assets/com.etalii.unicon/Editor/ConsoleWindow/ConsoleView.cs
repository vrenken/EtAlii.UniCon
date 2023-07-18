namespace EtAlii.UniCon.Editor
{
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UIElements;

    public partial class ConsoleView : VisualElement
    {
        private readonly ListView _listView;

        private readonly List<LogEventViewModel> _items = new ();

        private readonly Font _consoleFont = Resources.Load<Font>("Fonts/FiraCode-Regular");

        private readonly Button _tailButton;
        private readonly Color _tailButtonNonTrackingColor;
        private readonly Color _tailButtonTrackingColor;
        private readonly ScrollView _listViewScrollView;

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

            _tailButton = this.Q<Button>("tail-button");
            _tailButton.clicked += OnTailButtonClicked;
            _tailButtonNonTrackingColor = _tailButton.style.backgroundColor.value;
            _tailButtonTrackingColor = new Color(
                0.5f - _tailButtonNonTrackingColor.r, 
                0.5f - _tailButtonNonTrackingColor.g,
                0.5f - _tailButtonNonTrackingColor.b, 
                0.5f - _tailButtonNonTrackingColor.a);
            
            _listView = this.Q<ListView>();
            _listViewScrollView = _listView.Q<ScrollView>();
            _listViewScrollView.verticalScroller.valueChanged += OnScrolledVertically;
            _listView.itemsSource = _items;
            
            _listView.makeItem = () => new Foldout 
            { 
                style =
                {
                    flexGrow = 1,
                    unityFontDefinition = StyleKeyword.Initial,
                    unityFont = new StyleFont(_consoleFont)
                }, 
                value = false 
            };
            _listView.bindItem = (e, i) => Bind((Foldout)e, _items[i]);
        }

        private float _previousScrollValue;
        private ConsoleViewModel _viewModel;
        private CompositeDisposable _disposable;

        private void OnScrolledVertically(float value)
        {
            if (_isTrackingTail)
            {
                if (_previousScrollValue > _listViewScrollView.verticalScroller.value)
                {
                    OnTailButtonClicked();
                }
                _previousScrollValue = _listViewScrollView.verticalScroller.value;
            }
        }

        private void Bind(Foldout foldout, LogEventViewModel viewModel)
        {
            if (foldout.userData as LogEventViewModel != viewModel)
            {
                foldout.value = false;
                foldout.text = LogEventLine.GetMessage(viewModel);
                foldout.contentContainer.Clear();
                foldout.contentContainer.Add(BuildPropertyGrid(viewModel.LogEvent));
                foldout.userData = viewModel;
            }
        }
        
        public void Bind(ConsoleViewModel viewModel)
        {
            if (_disposable != null)
            {
                _disposable.Dispose();
            }
            _disposable = new CompositeDisposable();
            BindLogSourceSettings(viewModel, _disposable);
            BindLogLevelsSettings(viewModel, _disposable);
            
            if (_viewModel != null)
            {
                _viewModel.StreamChanged -= OnStreamChanged;
            }
            _viewModel = viewModel;
            _viewModel.StreamChanged += OnStreamChanged;
            OnStreamChanged();
        }
    }    
}
