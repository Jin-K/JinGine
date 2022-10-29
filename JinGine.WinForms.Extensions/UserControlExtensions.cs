using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace System.Windows.Forms;

public static class UserControlExtensions
{
    public static void InitArrowKeyDownFiring(this UserControl userControl)
    {
        userControl.PreviewKeyDown += (_, e) =>
        {
            if (e.KeyCode is Keys.Left or Keys.Right or Keys.Down or Keys.Up)
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
    public static void InitWin32Caret(this UserControl userControl, int width, int height, Func<int> xResolver, Func<int> yResolver)
    {
        void ResetCaretPos()
        {
            if (!Succeeded(SetCaretPos(xResolver(), yResolver())))
            {
                var unused = GetLastError();
            }
        }

        userControl.GotFocus += delegate
        {
            if (!Succeeded(CreateCaret(userControl.Handle, IntPtr.Zero, width, height)))
            {
                var unused = GetLastError();
            }
            ResetCaretPos();
            if (!Succeeded(ShowCaret(userControl.Handle)))
            {
                var unused = GetLastError();
            }
        };
        userControl.LostFocus += delegate
        {
            if (!Succeeded(DestroyCaret()))
            {
                var unused = GetLastError();
            }
        };
        userControl.Paint += delegate { ResetCaretPos(); };
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

    [DllImport("user32")]
    private static extern uint GetLastError();
}