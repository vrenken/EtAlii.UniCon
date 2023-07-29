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
            _viewModel.UserSettings.ClearOnPlay.Value = !_viewModel.UserSettings.ClearOnPlay.Value;
            action.UpdateActionStatus(action.eventInfo);
        }

        private DropdownMenuAction.Status OnClearOnPlayActionCallBack(DropdownMenuAction arg)
        {
            return _viewModel.UserSettings.ClearOnPlay.Value
                ? DropdownMenuAction.Status.Checked
                : DropdownMenuAction.Status.Normal;
        }

        private void OnClearOnBuildAction(DropdownMenuAction action)
        {
            _viewModel.UserSettings.ClearOnBuild.Value = !_viewModel.UserSettings.ClearOnBuild.Value;
            action.UpdateActionStatus(action.eventInfo);
        }

        private DropdownMenuAction.Status OnClearOnBuildActionCallBack(DropdownMenuAction arg)
        {
            return _viewModel.UserSettings.ClearOnBuild.Value
                ? DropdownMenuAction.Status.Checked
                : DropdownMenuAction.Status.Normal;
        }

        private void OnClearOnRecompileAction(DropdownMenuAction action)
        {
            _viewModel.UserSettings.ClearOnRecompile.Value = !_viewModel.UserSettings.ClearOnRecompile.Value;
            action.UpdateActionStatus(action.eventInfo);
        }
        
        private DropdownMenuAction.Status OnClearOnRecompileActionCallBack(DropdownMenuAction arg)
        {
            return _viewModel.UserSettings.ClearOnRecompile.Value
                ? DropdownMenuAction.Status.Checked
                : DropdownMenuAction.Status.Normal;
        }
    }    
}