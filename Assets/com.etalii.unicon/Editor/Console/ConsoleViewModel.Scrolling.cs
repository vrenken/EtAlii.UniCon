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
                UserSettings.ScrollToTail.Value = !UserSettings.ScrollToTail.Value;
            });
        }
    }    
}
