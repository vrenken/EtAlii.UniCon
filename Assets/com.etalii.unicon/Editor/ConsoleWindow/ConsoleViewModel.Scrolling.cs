namespace EtAlii.UniCon.Editor
{
    using System;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleViewModel
    {
        public readonly ReactiveCommand<ClickEvent> OnTailButtonClick = new();

        public event Action<string> ScrollingChanged;
        private void SetupScrolling()
        {
            OnTailButtonClick.Subscribe(_ =>
            {
                Settings.ScrollToTail = !Settings.ScrollToTail;
                ScrollingChanged?.Invoke(nameof(Settings.ScrollToTail));
            });
        }
    }    
}
