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
            
            _listView = this.Q<ListView>();
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
            _listView.ScrollToItem(-1);
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
