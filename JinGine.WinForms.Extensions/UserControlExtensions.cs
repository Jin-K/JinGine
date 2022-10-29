using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace System.Windows.Forms;

public static class UserControlExtensions
{
    public static void InitWin32Caret(this UserControl userControl, int width, int height, Func<int> xResolver, Func<int> yResolver)
    {
        void ResetCaretPos() => _ = SetCaretPos(xResolver(), yResolver());

        userControl.GotFocus += delegate
        {
            _ = CreateCaret(userControl.Handle, IntPtr.Zero, width, height);
            ResetCaretPos();
            _ = ShowCaret(userControl.Handle);
        };
        userControl.LostFocus += delegate { _ = DestroyCaret(); };
        userControl.Paint += delegate { ResetCaretPos(); };
    }

    [DllImport("user32")]
    private static extern int CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

    [DllImport("user32")]
    private static extern int DestroyCaret();

    [DllImport("user32")]
    private static extern int SetCaretPos(int x, int y);

    [DllImport("user32")]
    private static extern int ShowCaret(IntPtr hWnd);
}