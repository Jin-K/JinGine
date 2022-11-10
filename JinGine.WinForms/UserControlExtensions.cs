namespace JinGine.WinForms;

internal static class UserControlExtensions
{
    internal static void InitArrowKeyDownFiring(this UserControl userControl)
    {
        userControl.PreviewKeyDown += (_, e) =>
        {
            if (e.KeyCode is < Keys.Left or > Keys.Down) return;
            e.IsInputKey = true;
        };
    }

    internal static void InitMouseWheelScrollDelegation(this UserControl userControl, VScrollBar vScrollBar) =>
        userControl.MouseWheel += (_, e) =>
            vScrollBar.InvokeMouseWheel(e.Delta * SystemInformation.MouseWheelScrollLines);
}