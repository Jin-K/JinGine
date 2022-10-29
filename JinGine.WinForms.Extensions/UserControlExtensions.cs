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

    public static void InitMouseWheelScrolling(this UserControl userControl, VScrollBar vScrollBar)
    {
        userControl.MouseWheel += (_, e) =>
        {
            var deltaScroll = Math.Sign(e.Delta) * SystemInformation.MouseWheelScrollLines;
            if (deltaScroll is not 0)
            {
                var newScrollV = vScrollBar.Value - deltaScroll;
                if (newScrollV < 0 || newScrollV > vScrollBar.Maximum) return;
                vScrollBar.Value = newScrollV;
                userControl.Invalidate();
            }
        };
    }

    // TODO handle errors, convert them to exceptions
    // TODO make part of new Win32Caret type in Presentation layer
    public static void InitWin32Caret(this UserControl userControl, int width, int height, Func<Point> caretPointResolver)
    {
        userControl.GotFocus += delegate
        {
            if (!Succeeded(CreateCaret(userControl.Handle, IntPtr.Zero, width, height)))
            {
                var unused = Marshal.GetLastWin32Error();
            }
            var caretPoint = caretPointResolver();
            userControl.SetCaretPos(caretPoint.X, caretPoint.Y);
            if (!Succeeded(ShowCaret(userControl.Handle)))
            {
                var unused = Marshal.GetLastWin32Error();
            }
        };
        userControl.LostFocus += delegate
        {
            if (!Succeeded(DestroyCaret()))
            {
                var unused = Marshal.GetLastWin32Error();
            }
        };
        userControl.Paint += delegate
        {
            var caretPoint = caretPointResolver();
            userControl.SetCaretPos(caretPoint.X, caretPoint.Y);
        };
    }

    // TODO make part of new Win32Caret type in Presentation layer
    public static void SetCaretPos(this UserControl _, int x, int y)
    {
        if (!Succeeded(SetCaretPos(x, y)))
        {
            var unused = Marshal.GetLastWin32Error();
        }
    }

    private static bool Succeeded(int returnValue) => returnValue is not 0;

    [DllImport("user32", SetLastError = true)]
    private static extern int CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

    [DllImport("user32", SetLastError = true)]
    private static extern int DestroyCaret();

    [DllImport("user32", SetLastError = true)]
    private static extern int SetCaretPos(int x, int y);

    [DllImport("user32", SetLastError = true)]
    private static extern int ShowCaret(IntPtr hWnd);
}