using System.ComponentModel;
using System.Runtime.InteropServices;

namespace JinGine.WinForms.Controls.Helpers;

internal partial class Win32Caret
{
    private readonly UserControl _userControl;

    internal Point Position { get; set; }
    private Size Size { get; }

    internal Win32Caret(UserControl userControl, Size size)
    {
        _userControl = userControl;
        Size = size;

        userControl.GotFocus += OnGotFocus;
        userControl.LostFocus += OnLostFocus;
    }

    private void CreateCaret()
    {
        if (!Succeeded(CreateCaret(_userControl.Handle, IntPtr.Zero, Size.Width, Size.Height)))
            throw new Win32Exception(nameof(CreateCaret));

        _userControl.Paint += OnPaint;
    }

    private void OnGotFocus(object? sender, EventArgs e)
    {
        CreateCaret();
        SetCaretPos();
        ShowCaret();
    }

    private void OnLostFocus(object? sender, EventArgs e)
    {
        if (!Succeeded(DestroyCaret()))
            throw new Win32Exception(nameof(DestroyCaret));

        _userControl.Paint -= OnPaint;
    }

    private void OnPaint(object? sender, PaintEventArgs e)
    {
        SetCaretPos();
    }

    private void SetCaretPos()
    {
        if (!Succeeded(SetCaretPos(Position.X, Position.Y)))
            throw new Win32Exception(nameof(SetCaretPos));
    }

    private void ShowCaret()
    {
        if (!Succeeded(ShowCaret(_userControl.Handle)))
            throw new Win32Exception(nameof(ShowCaret));
    }

    private static bool Succeeded(int returnValue) => returnValue is not 0;

    [LibraryImport("user32", EntryPoint = nameof(CreateCaret), SetLastError = true)]
    private static partial int CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);
    
    [LibraryImport("user32", EntryPoint = nameof(DestroyCaret), SetLastError = true)]
    private static partial int DestroyCaret();

    [LibraryImport("user32", EntryPoint = nameof(SetCaretPos), SetLastError = true)]
    private static partial int SetCaretPos(int x, int y);

    [LibraryImport("user32", EntryPoint = nameof(ShowCaret), SetLastError = true)]
    private static partial int ShowCaret(IntPtr hWnd);
}