namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Collections.Generic;
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
        private readonly List<LogEvent> _items = new ();
        private readonly ScrollView _listViewScrollView;
        private readonly Button _metricsButton;
        private IDisposable _streamSubscription;
        private readonly Button _tailButton;

        private StreamingViewModel _viewModel;

        private float _previousScrollValue;
        private ExpressionViewModel _expressionViewModel;

        public StreamingView(VisualElement root)
        {
            //ColorUtility.TryParseHtmlString("#11ff1122", out var propertyGridHeaderColor);
            _propertyGridActionColor = Color.green * 0.85f;// propertyGridHeaderColor;

            _metricsButton = root.Q<ToolbarButton>("metrics-button");

            _tailButton = root.Q<Button>("tail-button");
            // Let's take the color of the tail button and use that to remember the toggled and not toggled colors.
            ToggleButtonExtension.Init(_tailButton);

            _listView = root.Q<ListView>();
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
            _listView.bindItem = (e, i) => BindFoldout((Foldout)e, _items[i]);
        }
        
        public void Bind(StreamingViewModel viewModel, ExpressionViewModel expressionViewModel, CompositeDisposable disposable)
        {
            _viewModel = viewModel;
            viewModel.StreamChanged += OnStreamChanged;

            _expressionViewModel = expressionViewModel;
            _tailButton
                .BindClick(viewModel.ToggleScrollToTail)
                .AddTo(disposable);
            _viewModel.UserSettings.ScrollToTail
                .Subscribe(onNext: scrollToTail =>
                {
                    _tailButton.UpdateToggleButton(scrollToTail);
                    ScrollWhenNeeded();
                })
                .AddTo(disposable);

            
            OnStreamChanged();
        }

        public void Unbind()
        {
            _viewModel.StreamChanged -= OnStreamChanged;
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

        private void OnStreamChanged()
        {
            if (_streamSubscription != null)
            {
                _streamSubscription.Dispose();
                _streamSubscription = null;
            }

            _items.Clear();
            _listView.Rebuild();

            _streamSubscription = _viewModel.Stream
                .Subscribe(onNext: Add);
        }

        private void Add(LogEvent logEvent)
        {
            _items.Add(logEvent);
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
            if (_viewModel.UserSettings.ScrollToTail.Value)
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
            if (!_viewModel.UserSettings.ScrollToTail.Value) return;
            if(_previousScrollValue == 0) return;
            if (_previousScrollValue <= _listViewScrollView.verticalScroller.value) return;
            
            _viewModel.UserSettings.ScrollToTail.Value = false;
            _tailButton.UpdateToggleButton(_viewModel.UserSettings.ScrollToTail.Value);
        }

    }    
}
