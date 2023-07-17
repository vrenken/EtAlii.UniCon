namespace EtAlii.UniCon.Editor
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using UnityEngine;
    using UnityEngine.UIElements;

    public partial class ConsoleView : VisualElement
    {
        private readonly ListView _listView;

        private readonly List<LogEventLineViewModel> _items = new ();

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

        private void Bind(Foldout foldout, LogEventLineViewModel viewModel)
        {
            if (foldout.userData as LogEventLineViewModel != viewModel)
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
            _listView.Clear();
            foreach (var item in viewModel.LogEvents)
            {
                Add(item);
            }
            viewModel.LogEvents.CollectionChanged += OnLogEventsChanged;
        }

        public void Unbind(ConsoleViewModel viewModel)
        {
            viewModel.LogEvents.CollectionChanged -= OnLogEventsChanged;
            _listView.Clear();
        }

        private void OnLogEventsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (LogEventLineViewModel item in e.NewItems)
                    {
                        Add(item);
                    }
                    break;
            }
        }

        private void Add(LogEventLineViewModel viewModel)
        {
            _items.Add(viewModel);
            _listView.RefreshItems();

            if (_isTrackingTail)
            {
                _listViewScrollView.verticalScroller.value = _listViewScrollView.verticalScroller.highValue > 0 ? _listViewScrollView.verticalScroller.highValue : 0;
                //_listViewScrollView.ScrollTo(_listViewScrollView...itemsSource[_listView.itemsSource.Count - 1]);
                //_listView.ScrollToItem(-1);
                //_listViewScrollView.verticalScroller.ScrollPageDown();
            }
        }
        
        //
        // private void RemoveEntry(VisualElement element)
        // {
        // }
        //
        // private VisualElement AddLogEntry()
        // {
        //     return new LogEventLine();
        // }
        //
        // private void BindLogEntry(VisualElement element, int index)
        // {
        //     var item = (Object)_items[index];
        //     element.Bind(new SerializedObject(item));
        //     element.userData = item;
        // }
        
        // private void UnbindEntry(VisualElement element, int index)
        // {
        //     element.Unbind();
        //     element.userData = null;
        // }

    }    
}
