namespace EtAlii.UniCon.Editor
{
    using System;
    using JetBrains.Annotations;
    using UniRx;
    using UnityEngine.UIElements;

    public abstract class View<T> : IView<T>
    {
        private VisualElement _templateRoot;

        public VisualElement TemplateRoot
        {
            get => _templateRoot;
            set
            {
                _templateRoot = value;
                Initialize();
            }
        }

        private CompositeDisposable _disposables;
        public CompositeDisposable Disposables => _disposables ??= new CompositeDisposable();

        protected abstract void Initialize();


        [MustUseReturnValue]
        public abstract IDisposable Bind(T viewModel);

        public void Dispose()
        {
            _disposables?.Dispose();
            _disposables = null;
        }
    }

    public static class ViewExtensions
    {
        public static T AddTo<T, TViewModel>(this T disposable, View<TViewModel> view) where T : IDisposable
        {
            view.Disposables.Add(disposable);
            return disposable;
        }

        public static void Add(this VisualElement element, IView view)
        {
            element.Add(view.TemplateRoot);
        }
    }
}