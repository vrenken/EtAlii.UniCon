namespace EtAlii.UniCon.Editor
{
    using UnityEngine.UIElements;

    public partial class ConsoleView
    {
        private void OnClearAction(DropdownMenuAction action) => _viewModel.Clear();

        private DropdownMenuAction.Status OnClearActionCallBack(DropdownMenuAction action)
        {
            return DropdownMenuAction.Status.Normal;
        }

        private void OnClearOnPlayAction(DropdownMenuAction action)
        {
            UserSettings.instance.ClearOnPlay.Value = !UserSettings.instance.ClearOnPlay.Value;
            action.UpdateActionStatus(action.eventInfo);
        }

        private DropdownMenuAction.Status OnClearOnPlayActionCallBack(DropdownMenuAction arg)
        {
            return UserSettings.instance.ClearOnPlay.Value
                ? DropdownMenuAction.Status.Checked | DropdownMenuAction.Status.Disabled
                : DropdownMenuAction.Status.Normal | DropdownMenuAction.Status.Disabled;
        }

        private void OnClearOnBuildAction(DropdownMenuAction action)
        {
            UserSettings.instance.ClearOnBuild.Value = !UserSettings.instance.ClearOnBuild.Value;
            action.UpdateActionStatus(action.eventInfo);
        }

        private DropdownMenuAction.Status OnClearOnBuildActionCallBack(DropdownMenuAction arg)
        {
            return UserSettings.instance.ClearOnBuild.Value
                ? DropdownMenuAction.Status.Checked | DropdownMenuAction.Status.Disabled
                : DropdownMenuAction.Status.Normal | DropdownMenuAction.Status.Disabled;
        }

        private void OnClearOnRecompileAction(DropdownMenuAction action)
        {
            UserSettings.instance.ClearOnRecompile.Value = !UserSettings.instance.ClearOnRecompile.Value;
            action.UpdateActionStatus(action.eventInfo);
        }
        
        private DropdownMenuAction.Status OnClearOnRecompileActionCallBack(DropdownMenuAction arg)
        {
            return UserSettings.instance.ClearOnRecompile.Value
                ? DropdownMenuAction.Status.Checked | DropdownMenuAction.Status.Disabled
                : DropdownMenuAction.Status.Normal | DropdownMenuAction.Status.Disabled;
        }
    }    
}