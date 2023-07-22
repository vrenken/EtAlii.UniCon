namespace EtAlii.UniCon.Editor
{
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleViewModel
    {
        public readonly ReactiveCommand<ClickEvent> OnFilterButtonClick = new();

        private void SetupFilter()
        {
            OnFilterButtonClick
                .Subscribe(_ =>
                {
                    Settings.ShowFilterPanel.Value = !Settings.ShowFilterPanel.Value;
                    Settings.FilterPanelWidth.SetValueAndForceNotify(Settings.FilterPanelWidth.Value);
                });

            Settings.UseSerilogSource.Subscribe(_ => ConfigureStream());
            Settings.UseUnitySource.Subscribe(_ => ConfigureStream());
            Settings.LogLevel.Subscribe(_ => ConfigureStream());
            Settings.ShowExceptions.Subscribe(_ => ConfigureStream());
        }
    }    
}
