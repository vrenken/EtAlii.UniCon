namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EtAlii.Unicon;
    using RedMoon.ReactiveKit;
    using Serilog.Events;
    using UniRx;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public partial class StreamingView
    {
        private readonly Font _consoleFont = Resources.Load<Font>("Fonts/FiraCode-Regular");

        private readonly ListView _listView;
        private readonly LinkedList<LogEntry> _items = new ();
        private readonly ScrollView _listViewScrollView;
        private readonly Button _metricsButton;
        private IDisposable _streamSubscription;
        private readonly Button _tailButton;

        private StreamingViewModel _viewModel;

        private float _previousScrollValue;
        private ExpressionViewModel _expressionViewModel;

        public StreamingView(VisualElement root)
        {
            _metricsButton = root.Q<ToolbarButton>("metrics-button");

            _tailButton = root.Q<Button>("tail-button");
            // Let's take the color of the tail button and use that to remember the toggled and not toggled colors.
            ToggleButtonExtension.Init(_tailButton);

            _listView = root.Q<ListView>();
            _listViewScrollView = _listView.Q<ScrollView>();
            _listViewScrollView.verticalScroller.valueChanged += OnScrolledVertically;
            _listView.itemsSource = new LinkedListWrapper(_items);
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
            _listView.bindItem = (e, i) => BindFoldout((Foldout)e, _items.Skip(i).First().LogEvent);
        }
        
        public void Bind(StreamingViewModel viewModel, ExpressionViewModel expressionViewModel, CompositeDisposable disposable)
        {
            _viewModel = viewModel;

            _expressionViewModel = expressionViewModel;
            _tailButton
                .BindClick(viewModel.ToggleScrollToTail)
                .AddTo(disposable);
            UserSettings.instance.ScrollToTail
                .Subscribe(onNext: scrollToTail =>
                {
                    _tailButton.UpdateToggleButton(scrollToTail);
                    ScrollWhenNeeded();
                })
                .AddTo(disposable);
            _viewModel.Stream
                .Subscribe(stream =>
                {
                    _streamSubscription?.Dispose();

                    _items.Clear();
                    _listView.Rebuild();

                    UpdateMetrics();
                    
                    _streamSubscription = stream
                        .ObserveOnMainThread()
                        //.SubscribeOnMainThread()
                        .Subscribe(onNext: Add);
                })
                .AddTo(disposable);
        }

        public void Unbind()
        {
        }

        private void BindFoldout(Foldout foldout, LogEvent logEvent)
        {
            if (foldout.userData == logEvent) return;
            foldout.value = false;
            foldout.text = LogEventLine.GetMessage(logEvent);
            foldout.contentContainer.Clear();
            foldout.contentContainer.Add(BuildPropertyGrid(logEvent));
            foldout.userData = logEvent;
        }

        private void Add(LogEntry logEntry)
        {
            _items.AddLast(logEntry);
            _listView.RefreshItems();

            UpdateMetrics();
            ScrollWhenNeeded();
        }

        private void UpdateMetrics()
        {
            var total = LogSink.Instance.EventCount;
            var filtered = _items.Count;

            _metricsButton.text = $"Filtered: {Format(filtered)} / Total: {Format(total)}";
        }

        private string Format(int number)
        {
            return number switch
            {
                > 1000000 => $"{number / 1000000:D}M",
                > 1000 => $"{number / 1000:D}K",
                _ => $"{number}"
            };
        }
        
        private void ScrollWhenNeeded()
        {
            if (UserSettings.instance.ScrollToTail.Value)
            {
                _listView.ScrollToItem(-1);
                _listViewScrollView.verticalScroller.value = _listViewScrollView.verticalScroller.highValue;
                _previousScrollValue = float.IsNaN(_listViewScrollView.contentRect.height) ? 0 : _listViewScrollView.verticalScroller.value;
            }
        }
        
        /// <summary>
        /// When tail tracking is active We need to check if the user scrolled upwards.
        /// In that case we stop the tail tracking.  
        /// </summary>
        /// <param name="value"></param>
        private void OnScrolledVertically(float value)
        {
            if (!UserSettings.instance.ScrollToTail.Value) return;
            if(_previousScrollValue == 0) return;
            if (_previousScrollValue <= _listViewScrollView.verticalScroller.value) return;
            
            UserSettings.instance.ScrollToTail.Value = false;
            _tailButton.UpdateToggleButton(UserSettings.instance.ScrollToTail.Value);
        }
    }    
}
