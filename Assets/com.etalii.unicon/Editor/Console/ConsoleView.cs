namespace EtAlii.UniCon.Editor
{
    using System.Collections.Generic;
    using Serilog.Events;
    using UniRx;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public partial class ConsoleView : VisualElement
    {
        private readonly ListView _listView;

        private readonly List<LogEvent> _items = new ();

        private readonly Font _consoleFont = Resources.Load<Font>("Fonts/FiraCode-Regular");

        private readonly Color _buttonNotToggledColor;
        private readonly Color _buttonToggledColor;
        private readonly ScrollView _listViewScrollView;
        private readonly ToolbarMenu _expressionAddButtonMenu;

        private readonly Button _metricsButton;
        
        private float _previousScrollValue;
        private ConsoleViewModel _viewModel;
        private CompositeDisposable _disposables;
        private readonly TwoPaneSplitView _horizontalSplitPanel;
        private readonly TwoPaneSplitView _verticalSplitPanel;

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
            //ColorUtility.TryParseHtmlString("#11ff1122", out var propertyGridHeaderColor);
            _propertyGridActionColor = Color.green * 0.85f;// propertyGridHeaderColor;

            var visualTree = Resources.Load<VisualTreeAsset>(nameof(ConsoleView));
            visualTree.CloneTree(this);

            _horizontalSplitPanel = this.Q<TwoPaneSplitView>("horizontal-split-panel");
            _verticalSplitPanel = this.Q<TwoPaneSplitView>("vertical-split-panel");

            _metricsButton = this.Q<ToolbarButton>("metrics-button");
            _filterPanel = this.Q<VisualElement>("filter-panel");
            _filterButton = this.Q<Button>("filter-button");
            _customFiltersFoldout = this.Q<Foldout>("custom-filters-foldout");
            
            _expressionPanel = this.Q<VisualElement>("expression-panel");
            _expressionButton = this.Q<Button>("expression-button");
            _expressionTextField = this.Q<TextField>("expression-textfield");
            _expressionErrorButton = this.Q<Button>("expression-error-button");
            _expressionSaveButton = this.Q<Button>("expression-save-button");
            
            _tailButton = this.Q<Button>("tail-button");

            // Let's take the color of the tail button and use that to remember the toggled and not toggled colors.
            _buttonNotToggledColor = _tailButton.style.backgroundColor.value;
            _buttonToggledColor = new Color(
                0.5f - _buttonNotToggledColor.r, 
                0.5f - _buttonNotToggledColor.g,
                0.5f - _buttonNotToggledColor.b, 
                0.5f - _buttonNotToggledColor.a);
            
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

        private void Bind(Foldout foldout, LogEvent logEvent)
        {
            if (foldout.userData == logEvent) return;
            foldout.value = false;
            foldout.text = LogEventLine.GetMessage(logEvent);
            foldout.contentContainer.Clear();
            foldout.contentContainer.Add(BuildPropertyGrid(logEvent));
            foldout.userData = logEvent;
        }
        
        public void Bind(ConsoleViewModel viewModel)
        {
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
            
            if (_viewModel != null)
            {
                _viewModel.StreamChanged -= OnStreamChanged;
                _viewModel.ExpressionChanged -= OnExpressionChanged;
            }
            _viewModel = viewModel;

            BindScrolling(viewModel, _disposables);
            BindFilter(viewModel, _disposables);
            BindExpression(viewModel, _disposables);
            
            _viewModel.ExpressionChanged += OnExpressionChanged;
            _viewModel.StreamChanged += OnStreamChanged;
            OnStreamChanged();
        }
        
        private void UpdateToggleButton(Button button, bool isToggled)
        {
            button.style.backgroundColor = isToggled 
                ? _buttonToggledColor 
                : _buttonNotToggledColor;
        }
    }    
}
