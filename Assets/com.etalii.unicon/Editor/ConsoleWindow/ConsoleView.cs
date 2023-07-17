namespace EtAlii.UniCon.Editor
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class ConsoleView : VisualElement
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
            
            _listView.makeItem = () => new Foldout { style =
            {
                flexGrow = 1,
                unityFontDefinition = StyleKeyword.Initial,
                unityFont = new StyleFont(_consoleFont)
            }, value = false };
            _listView.bindItem = (e, i) => Bind((Foldout)e, _items[i]);
            _listView.unbindItem = (e, _) => Unbind((Foldout)e);
        }

        private void Unbind(Foldout foldout)
        {
            foldout.contentContainer.Clear();
        }

        private void Bind(Foldout foldout, LogEventLineViewModel viewModel)
        {
            foldout.text = LogEventLine.GetMessage(viewModel);
            foldout.contentContainer.Add(new Label
            {
                style = { flexGrow = 1, whiteSpace = WhiteSpace.Normal }, 
                text = "Lorem ipsum dolor sit amet. In rerum sapiente ea voluptas officia qui internos rerum est optio unde et Quis autem et deleniti architecto in perferendis reprehenderit! Ut sunt inventore sit nemo unde est magni sequi ut incidunt maiores. Et excepturi dolor cum obcaecati autem vel beatae error ea voluptas quia aut praesentium autem qui officiis ullam est obcaecati voluptatem."
            });
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
