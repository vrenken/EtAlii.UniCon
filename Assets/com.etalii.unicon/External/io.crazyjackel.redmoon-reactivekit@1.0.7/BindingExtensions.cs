﻿// ReSharper disable all 
namespace RedMoon.ReactiveKit
{
    using JetBrains.Annotations;
    using System;
    using UniRx;
    using UnityEngine.UIElements;

    internal static class BindingExtensions
    {
        /// <summary>
        /// Binds Element's Enabled State to Boolean Observable
        /// </summary>
        /// <param name="element">Element to Bind</param>
        /// <param name="observable">Observable Stream to Subscribe to</param>
        /// <returns>Disposable for Disposing Subscription</returns>
        [MustUseReturnValue]
        public static IDisposable BindEnabled(this VisualElement element, IObservable<bool> observable)
        {
            return observable.ObserveOnMainThread().DistinctUntilChanged().Subscribe(element.SetEnabled);
        }
        /// <summary>
        /// Binds Element's Visibility State to Visibility Observable
        /// </summary>
        /// <param name="element">Element to Bind</param>
        /// <param name="observable">Observable Stream to Subscribe to</param>
        /// <returns>Disposable for Disposing Subscription</returns>
        [MustUseReturnValue]
        public static IDisposable BindVisibility(this VisualElement element, IObservable<Visibility> observable)
        {
            return observable
                .ObserveOnMainThread()
                .DistinctUntilChanged()
                .Subscribe(value => element.style.visibility = value);
        }
        
        /// <summary>
        /// Binds Element's Visibility State to Visibility Observable
        /// </summary>
        /// <param name="element">Element to Bind</param>
        /// <param name="observable">Observable Stream to Subscribe to</param>
        /// <returns>Disposable for Disposing Subscription</returns>
        [MustUseReturnValue]
        public static IDisposable BindVisibility(this VisualElement element, IObservable<bool> observable)
        {
            return observable
                .ObserveOnMainThread()
                .DistinctUntilChanged()
                .Subscribe(value => element.style.visibility = value ? Visibility.Visible : Visibility.Hidden);
        }
        
        /// <summary>
        /// Bind Element's Click Callback to Command
        /// </summary>
        /// <param name="element">Element to Bind</param>
        /// <param name="command">Command to Execute</param>
        /// <returns>Disposable for Disposing Subscription</returns>
        [MustUseReturnValue]
        public static IDisposable BindClick(this VisualElement element, IReactiveCommand<ClickEvent> command)
        {
            IDisposable d1 = element.BindEnabled(command.CanExecute);
            IDisposable d2 = element.BindCallback(command);
            return StableCompositeDisposable.Create(d1, d2);
        }
        /// <summary>
        /// Bind Element's Click Callback to Command
        /// </summary>
        /// <param name="element">Element to Bind</param>
        /// <param name="command">Command to Execute</param>
        /// <param name="dataForCallback">Callback as a part of Command</param>
        /// <returns>Disposable for Disposing Subscription</returns>
        [MustUseReturnValue]
        public static IDisposable BindClick<TArgs>(this VisualElement element, IReactiveCommand<(ClickEvent, TArgs)> command, TArgs dataForCallback)
        {
            var d1 = element.BindEnabled(command.CanExecute);
            var d2 = element.BindCallback(command, dataForCallback);
            return StableCompositeDisposable.Create(d1, d2);
        }

        [MustUseReturnValue]
        public static IDisposable BindCallback<TEventType>(this CallbackEventHandler element, EventCallback<TEventType> callback, TrickleDown trickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
        {
            element.RegisterCallback(callback, trickleDown);
            return Disposable.Create(() => { element.UnregisterCallback(callback, trickleDown); });
        }
        [MustUseReturnValue]
        public static IDisposable BindCallback<TEventType, TUserArgsType>(this CallbackEventHandler element, EventCallback<TEventType, TUserArgsType> callback, TUserArgsType dataForCallback, TrickleDown trickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
        {
            element.RegisterCallback(callback, dataForCallback, trickleDown);
            return Disposable.Create(() => { element.UnregisterCallback(callback, trickleDown); });
        }
        [MustUseReturnValue]
        public static IDisposable BindCallback<TEventType>(this CallbackEventHandler element, IReactiveCommand<TEventType> command, TrickleDown trickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
        {
            EventCallback<TEventType> callback = new EventCallback<TEventType>((ev) => command.Execute(ev));
            element.RegisterCallback(callback, trickleDown);
            return Disposable.Create(() => { element.UnregisterCallback(callback, trickleDown); });
        }
        [MustUseReturnValue]
        public static IDisposable BindCallback<TEventType, TUserArgsType>(this CallbackEventHandler element, IReactiveCommand<(TEventType, TUserArgsType)> command, TUserArgsType dataForCallback, TrickleDown trickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
        {
            EventCallback<TEventType, TUserArgsType> callback = new EventCallback<TEventType, TUserArgsType>((ev, args) => command.Execute((ev, args)));
            element.RegisterCallback(callback, dataForCallback, trickleDown);
            return Disposable.Create(() => { element.UnregisterCallback(callback, trickleDown); });
        }

        [MustUseReturnValue]
        public static IDisposable BindValueChanged<T>(this INotifyValueChanged<T> element, IReactiveCommand<ChangeEvent<T>> command)
        {
            EventCallback<ChangeEvent<T>> callback = new EventCallback<ChangeEvent<T>>((ev) => command.Execute(ev));
            element.RegisterValueChangedCallback(callback);
            return Disposable.Create(() => { element.UnregisterValueChangedCallback(callback); });
        }
        /// <summary>
        /// Binds a Property Value to Element Changing
        /// </summary>
        /// <typeparam name="T">Type for Change Event</typeparam>
        /// <param name="element">Element that Notifies Change</param>
        /// <param name="property">Property to Change on Notification</param>
        /// <returns>Disposable for Disposing Subscription</returns>
        [MustUseReturnValue]
        public static IDisposable BindValueChanged<T>(this INotifyValueChanged<T> element, IReactiveProperty<T> property)
        {
            EventCallback<ChangeEvent<T>> callback = new EventCallback<ChangeEvent<T>>((ev) => property.Value = ev.newValue);
            element.RegisterValueChangedCallback(callback);
            return Disposable.Create(() => { element.UnregisterValueChangedCallback(callback); });
        }
        /// <summary>
        /// Binds an Element to a Property Changing
        /// Does not Trigger Element Notifications
        /// </summary>
        /// <typeparam name="T">Type for Fields</typeparam>
        /// <param name="element">Element that has value updated</param>
        /// <param name="observable">Observable to update value</param>
        /// <returns>Disposable for Disposing Subscription</returns>
        [MustUseReturnValue]
        public static IDisposable BindToValueChanged<T>(this INotifyValueChanged<T> element, IObservable<T> observable)
        {
            return observable
                .ObserveOnMainThread()
                .DistinctUntilChanged()
                .Subscribe((ev) => element.SetValueWithoutNotify(ev));
        }

        [MustUseReturnValue]
        public static IDisposable BindTwoWayValueChanged<T>(this INotifyValueChanged<T> element, IReactiveProperty<T> property)
        {
            IDisposable d1 = BindValueChanged(element, property);
            IDisposable d2 = BindToValueChanged(element, property);
            return StableCompositeDisposable.Create(d1, d2);
        }

        [MustUseReturnValue]
        public static IDisposable BindTwoWayValueChanged<T>(this INotifyValueChanged<bool> element, IReactiveProperty<T> property, T value)
            where T: Enum
        {
            return BindTwoWayValueChanged<bool, T>(element, property, ll => ll.HasFlag(value), (v, u) =>
            {
                var uv = Convert.ToInt32(u);
                var valuev = Convert.ToInt32(value);
                var newValue = v
                        ? uv | valuev
                        : uv & ~valuev;
                return (T)Enum.ToObject(typeof(T), newValue);
            });
        }

        [MustUseReturnValue]
        public static IDisposable BindTwoWayValueChanged<T, U>(this INotifyValueChanged<T> element, IReactiveProperty<U> property, Func<U, T> selector1, Func<T, U, U> selector2)
        {
            EventCallback<ChangeEvent<T>> callback = new EventCallback<ChangeEvent<T>>((ev) =>
            {
                property.Value = selector2(ev.newValue, property.Value);
            });
            element.RegisterValueChangedCallback(callback);
            var d1 = Disposable.Create(() => { element.UnregisterValueChangedCallback(callback); });
            var d2 = property
                .DistinctUntilChanged()
                .Select(selector1)
                .ObserveOnMainThread()
                .Subscribe((ev) => element.SetValueWithoutNotify(ev));
            return StableCompositeDisposable.Create(d1, d2);
        }
    }
}