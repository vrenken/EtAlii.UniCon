namespace EtAlii.UniCon.Editor
{
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleViewModel
    {
        public readonly ReactiveCommand<ClickEvent> OnTailButtonClick = new();

        private void SetupScrolling()
        {
            OnTailButtonClick.Subscribe(_ =>
            {
                Settings.ScrollToTail.Value = !Settings.ScrollToTail.Value;
            });
        }
    }    
}
