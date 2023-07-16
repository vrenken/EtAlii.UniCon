namespace EtAlii.UniCon.Editor
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class ConsoleView : VisualElement
    {
        private readonly MultiColumnListView _listView;

        private readonly List<LogEventLineViewModel> _items = new ();
        
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
            
            _listView = this.Q<MultiColumnListView>();
            _listView.itemsSource = _items;
            
            // Make
            _listView.columns["timestamp"].makeCell = () => new Label { style = { unityTextAlign = TextAnchor.MiddleLeft }};
            _listView.columns["log-level"].makeCell = () => new Label { style = { unityTextAlign = TextAnchor.MiddleLeft }};
            _listView.columns["message"].makeCell = () => new Label { enableRichText = true, style = { unityTextAlign = TextAnchor.MiddleLeft }};
            
            _listView.columns["timestamp"].bindCell = (e, i) => ((Label)e).text = _items[i].timestamp;
            _listView.columns["log-level"].bindCell = (e, i) => ((Label)e).text = _items[i].level;
            _listView.columns["message"].bindCell = (e, i) => ((Label)e).text = BuildMessage(_items[i]);
        }

        private string BuildMessage(LogEventLineViewModel viewModel)
        {
            return MarkerMessageTemplateRenderer.Render(viewModel.LogEvent.MessageTemplate, viewModel.LogEvent.Properties);
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
