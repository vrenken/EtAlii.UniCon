﻿namespace EtAlii.UniCon.Editor
{
    using System.Collections.Generic;
    using Serilog.Events;
    using UniRx;
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

            _rightPanelGroup = this.Q<TwoPaneSplitView>("right-panel-group");
            
            _filterPanel = this.Q<VisualElement>("filter-panel");
            _filterButton = this.Q<Button>("filter-button");
            
            _rulesPanel = this.Q<VisualElement>("rules-panel");
            _rulesButton = this.Q<Button>("rules-button");
            _rulesList = this.Q<ScrollView>("rules-list");
            
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

        private float _previousScrollValue;
        private ConsoleViewModel _viewModel;
        private CompositeDisposable _disposable;
        private readonly TwoPaneSplitView _rightPanelGroup;

        private void OnScrolledVertically(float value)
        {
            if (!_viewModel.Settings.ScrollToTail) return;
            if (_previousScrollValue <= _listViewScrollView.verticalScroller.value) return;
            
            _viewModel.Settings.ScrollToTail = false;
            UpdateToggleButton(_tailButton, _viewModel.Settings.ScrollToTail);
        }

        private void Bind(Foldout foldout, LogEvent logEvent)
        {
            if (foldout.userData as LogEvent == logEvent) return;
            
            foldout.value = false;
            foldout.text = LogEventLine.GetMessage(logEvent);
            foldout.contentContainer.Clear();
            foldout.contentContainer.Add(BuildPropertyGrid(logEvent));
            foldout.userData = logEvent;
        }
        
        public void Bind(ConsoleViewModel viewModel)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            
            if (_viewModel != null)
            {
                _viewModel.StreamChanged -= OnStreamChanged;
                _viewModel.ScrollingChanged -= OnScrollingChanged;
                _viewModel.FilterChanged -= OnFilterChanged;
                _viewModel.RulesChanged -= OnRulesChanged;
            }
            _viewModel = viewModel;
            
            BindScrolling(viewModel, _disposable);
            BindFilter(viewModel, _disposable);
            BindRules(viewModel, _disposable);
            
            _viewModel.FilterChanged += OnFilterChanged;
            _viewModel.RulesChanged += OnRulesChanged;
            _viewModel.ScrollingChanged += OnScrollingChanged;
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
