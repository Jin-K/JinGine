﻿using System.ComponentModel;
using System.Runtime.InteropServices;

namespace JinGine.WinForms.Controls.Helpers;

internal class Win32Caret
{
    private readonly UserControl _userControl;

    internal Point Position { get; set; }
    internal Size Size { get; set; }

    internal Win32Caret(UserControl userControl)
    {
        _userControl = userControl;

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

    [DllImport("user32", SetLastError = true)]
    private static extern int CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

    [DllImport("user32", SetLastError = true)]
    private static extern int DestroyCaret();

    [DllImport("user32", SetLastError = true)]
    private static extern int SetCaretPos(int x, int y);

    [DllImport("user32", SetLastError = true)]
    private static extern int ShowCaret(IntPtr hWnd);
}