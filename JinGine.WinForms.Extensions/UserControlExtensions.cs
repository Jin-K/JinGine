using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace System.Windows.Forms;

public static class UserControlExtensions
{
    public static void InitArrowKeyDownFiring(this UserControl userControl)
    {
        userControl.PreviewKeyDown += (_, e) =>
        {
            if (e.KeyCode is < Keys.Left or > Keys.Down) return;
            e.IsInputKey = true;
        };
    }

    public static void InitMouseWheelScrollDelegation(this UserControl userControl, VScrollBar vScrollBar)
    {
        userControl.MouseWheel += (_, e) =>
        {
            var direction = Math.Sign(e.Delta);
            if (direction is 0) return;
            
            // ReSharper disable InconsistentNaming
            const ushort WM_MOUSEWHEEL = 0x020a;
            const ushort MK_LBUTTON = 0x0001;
            // ReSharper restore InconsistentNaming

            var delta = SystemInformation.MouseWheelScrollDelta * SystemInformation.MouseWheelScrollLines;
            var wParam = (IntPtr)((delta << 16) * Math.Sign(direction) | MK_LBUTTON);
            var lParam = (IntPtr)((Cursor.Position.Y << 16) | (Cursor.Position.X & 0xffff));

            SendMessage(vScrollBar.Handle, WM_MOUSEWHEEL, wParam, lParam);
        };
    }

    [DllImport("user32", SetLastError = true)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}