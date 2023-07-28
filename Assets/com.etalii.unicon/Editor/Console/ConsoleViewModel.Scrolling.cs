namespace EtAlii.UniCon.Editor
{
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleViewModel
    {
        public readonly ReactiveCommand<ClickEvent> ToggleScrollToTail = new();

        private void SetupScrolling()
        {
            ToggleScrollToTail.Subscribe(_ =>
            {
                UserSettings.ScrollToTail.Value = !UserSettings.ScrollToTail.Value;
            });
        }
    }    
}
