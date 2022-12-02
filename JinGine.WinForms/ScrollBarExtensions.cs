using System.Runtime.InteropServices;

namespace JinGine.WinForms;

internal static partial class ScrollBarExtensions
{
    internal static void InvokeMouseWheel(this ScrollBar scrollBar, int delta)
    {
        const ushort wmMousewheel = 0x020a; // WM_MOUSEWHEEL
        const ushort mkLbutton = 0x0001; // MK_LBUTTON
        var wParam = (IntPtr)((delta << 16) | mkLbutton);
        var lParam = (IntPtr)((Cursor.Position.Y << 16) | (Cursor.Position.X & 0xffff));
        SendMessage(scrollBar.Handle, wmMousewheel, wParam, lParam);
    }

    [LibraryImport("user32", EntryPoint = "SendMessageA", SetLastError = true)]
    private static partial void SendMessage(nint hWnd, uint msg, nint wParam, nint lParam);
}