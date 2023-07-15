namespace EtAlii.UniCon.Editor
{
    using System.Threading;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UIElements;
    
    /**
     * This is a collection of binding extensions for UI Toolkit using UniRx
     *
     * The SmartObserveOnMainThread() is similar to ObserveOnMainThread(), except that it checks, if the current
     * thread is already the main thread and if so, it will execute on it. This has the advantage, that changes to the
     * UI originating from user input/the main thread won't have any delay. If all changes originate from the main
     * thread anyways, it can safely be removed.
     */
    public static class UiToolkitBindingExtensions
    {
        static readonly Thread MainThread = Thread.CurrentThread;

        private static IObservable<T> SmartObserveOnMainThread<T>(this IObservable<T> observable)
        {
            return Thread.CurrentThread != MainThread 
                ? observable.ObserveOnMainThread()
                : observable;
        }

        [MustUseReturnValue]
        public static IDisposable BindEnabled(this VisualElement element, IObservable<bool> observable)
        {
            return observable.SmartObserveOnMainThread().DistinctUntilChanged().Subscribe(element.SetEnabled);
        }

        [MustUseReturnValue]
        public static IDisposable BindVisibility(this VisualElement element, IObservable<bool> observable)
        {
            return observable.SmartObserveOnMainThread().DistinctUntilChanged().Subscribe(s => element.visible = s);
        }

        [MustUseReturnValue]
        public static IDisposable BindWidth(this VisualElement element, IObservable<StyleLength> width)
        {
            return width.SmartObserveOnMainThread().Subscribe(value => element.style.width = value);
        }

        // [MustUseReturnValue]
        // public static IDisposable BindDisplay(this VisualElement visualElement, IObservable<bool> flex)
        // {
        //     return flex.SmartObserveOnMainThread()
        //         .DistinctUntilChanged()
        //         .Subscribe(visualElement.SetDisplay);
        // }

        [MustUseReturnValue]
        public static IDisposable Bind(this TextElement textElement, IObservable<string> observable)
        {
            return observable.SmartObserveOnMainThread().Subscribe(value => textElement.text = value);
        }

        [MustUseReturnValue]
        public static IDisposable Bind(this Button button, Action action)
        {
            return button.ClickedAsObservable().Subscribe(_ =>
            {
                if (button.enabledSelf)
                {
                    action();
                }
            });
        }

        [MustUseReturnValue]
        public static IDisposable Bind(this Button button, Func<Task> asyncAction)
        {
            return button.ClickedAsObservable().Subscribe(_ =>
            {
                if (button.enabledSelf)
                {
                    MainThreadDispatcher.StartCoroutine(RunAsync(asyncAction));
                }
            });
        }

        private static IEnumerator RunAsync(Func<Task> asyncAction)
        {
            var task = asyncAction();
            yield return new WaitUntil(() => task.IsCompleted);
        }


        [MustUseReturnValue]
        public static IDisposable BindMouseDown(this Button button, Action action)
        {
            return button.MouseDownEventAsObservable().Subscribe(_ =>
            {
                if (button.enabledSelf)
                {
                    action();
                }
            });
        }

        // [MustUseReturnValue]
        // public static IDisposable BindHoldDown(this Button button, Action action)
        // {
        //     return Observable
        //         .FromEvent(h => button.clickable = new PointerClickable(h, 0, 1), h => button.clicked -= h)
        //         .Subscribe(_ =>
        //         {
        //             if (button.enabledSelf)
        //             {
        //                 action();
        //             }
        //         });
        // }

        [MustUseReturnValue]
        public static IDisposable BindLeftClick(this VisualElement visualElement, Action action)
        {
            return visualElement.AsObservable<ClickEvent>().Where(evt => evt.button == 0).Subscribe(_ => action());
        }

        // [MustUseReturnValue]
        // public static IDisposable BindRightClick(this VisualElement visualElement, Action action)
        // {
        //     return new CompositeDisposable
        //     {
        //         // For some reason, the ClickEvent does not work here, so the MouseUpEvent is used
        //         visualElement.AsObservable<MouseUpEvent>().Where(evt => evt.button == 1)
        //             .Subscribe(_ => action()),
        //         visualElement.DisableRightClickForInputManager()
        //     };
        // }

        [MustUseReturnValue]
        public static IDisposable BindClass(this VisualElement visualElement,
            IObservable<Tuple<string, bool>> className)
        {
            return className.SmartObserveOnMainThread()
                .Subscribe(value => visualElement.EnableInClassList(value.Item1, value.Item2));
        }

        // [MustUseReturnValue]
        // public static IDisposable BindClass(this VisualElement visualElement,
        //     IObservable<string> className, Predicate<string> predicateToRemove)
        // {
        //     return className.SmartObserveOnMainThread()
        //         .Subscribe(value =>
        //         {
        //             visualElement.RemoveFromClassList(predicateToRemove);
        //             visualElement.AddToClassList(value);
        //         });
        // }

        [MustUseReturnValue]
        public static IDisposable BindClasses(this VisualElement visualElement,
            IObservable<IEnumerable<string>> classNames)
        {
            return classNames.SmartObserveOnMainThread()
                .Subscribe(value =>
                {
                    visualElement.ClearClassList();
                    foreach (var className in value)
                    {
                        visualElement.AddToClassList(className);
                    }
                });
        }

        [MustUseReturnValue]
        public static IDisposable BindStyle<T>(this VisualElement visualElement, IObservable<T> styleValue,
            Action<VisualElement, T> styleAction)
        {
            return styleValue.SmartObserveOnMainThread()
                .Subscribe(value => styleAction(visualElement, value));
        }


        [MustUseReturnValue]
        public static IDisposable BindTopInPercent(this VisualElement visualElement, IObservable<float> top)
        {
            return visualElement.BindStyle(top, (element, value) =>
                element.style.top = new StyleLength(new Length(value * 100, LengthUnit.Percent)));
        }

        [MustUseReturnValue]
        public static IDisposable BindLeftInPercent(this VisualElement visualElement, IObservable<float> left)
        {
            return visualElement.BindStyle(left, (element, value) =>
                element.style.left = new StyleLength(new Length(value * 100, LengthUnit.Percent)));
        }

        // [MustUseReturnValue]
        // public static IDisposable BindRotation(this VisualElement visualElement,
        //     IObservable<float> angleInDegree)
        // {
        //     return angleInDegree.SmartObserveOnMainThread()
        //         .Subscribe(visualElement.SetRotation);
        // }

        [MustUseReturnValue]
        public static IDisposable BindClasses(this VisualElement visualElement,
            IObservable<IEnumerable<Tuple<string, bool>>> classNames)
        {
            return classNames.SmartObserveOnMainThread()
                .Subscribe(values =>
                {
                    foreach (var (className, addOrRemove) in values)
                    {
                        visualElement.EnableInClassList(className, addOrRemove);
                    }
                });
        }

        [MustUseReturnValue]
        public static IDisposable BindHorizontalScrolling(this Button button, ScrollView scrollView,
            float scrollOffset)
        {
            return button.Bind(() =>
            {
                var scroller = scrollView.horizontalScroller;
                var value = Mathf.Clamp(scroller.value + scrollOffset, scroller.lowValue, scroller.highValue);

                scrollView.scrollOffset = new Vector2(value, scrollView.scrollOffset.y);
            });
        }

        // [MustUseReturnValue]
        // public static IDisposable Bind(this VideoPlayer player, VideoClip clip)
        // {
        //     player.clip = clip;
        //     player.Play();
        //     return new ActionDisposable(() =>
        //     {
        //         player.Stop();
        //         player.clip = null;
        //         player.targetTexture.Release();
        //     });
        // }

        // [MustUseReturnValue]
        // public static IDisposable DisableRightClickForInputManager(this VisualElement element)
        // {
        //     return element.OnMouseOverAsObservable().Subscribe(value => InputManager.RightClickDisabled = value);
        // }

        public static IObservable<bool> OnMouseOverAsObservable(this VisualElement element)
        {
            return element.AsObservable<MouseOverEvent>().Select(evt => true)
                .Merge(element.AsObservable<MouseOutEvent>().Select(evt => false))
                .Merge(element.AsObservable<DetachFromPanelEvent>().Select(evt => false));
        }

        private static IObservable<TEventType> AsObservable<TEventType>(this CallbackEventHandler handler)
            where TEventType : EventBase<TEventType>, new()
        {
            return Observable.FromEvent<EventCallback<TEventType>, TEventType>(h => evt => h(evt),
                h => handler.RegisterCallback(h),
                h => handler.UnregisterCallback(h));
        }

        private static IObservable<Unit> MouseDownEventAsObservable(this Button button)
        {
            button.clickable.activators.Clear();
            return button.AsObservable<MouseDownEvent>().Where(evt => evt.button == 0).Select(_ => Unit.Default);
        }

        private static IObservable<Unit> ClickedAsObservable(this Button button)
        {
            return Observable.FromEvent<Action, Unit>(h => () => h(Unit.Default),
                h => button.clicked += h,
                h => button.clicked -= h);
        }
    }
}