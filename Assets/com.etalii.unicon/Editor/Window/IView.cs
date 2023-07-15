namespace EtAlii.UniCon.Editor
{
    using System;
    using UnityEngine.UIElements;

    public interface IView<in TViewModel> : IView
    {
        IDisposable Bind(TViewModel viewModel);
    }

    public interface IView : IDisposable
    {
        VisualElement TemplateRoot { get; set; }
    }
}