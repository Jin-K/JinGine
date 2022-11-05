using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace System.Windows.Forms;

public static class ScrollBarExtensions
{
    public static void RaiseMouseWheel(this ScrollBar scrollBar, int delta)
    {
        const ushort wmMousewheel = 0x020a; // WM_MOUSEWHEEL
        const ushort mkLbutton = 0x0001; // MK_LBUTTON
        var wParam = (IntPtr)((delta << 16) | mkLbutton);
        var lParam = (IntPtr)((Cursor.Position.Y << 16) | (Cursor.Position.X & 0xffff));
        SendMessage(scrollBar.Handle, wmMousewheel, wParam, lParam);
    }

    [DllImport("user32", SetLastError = true)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}