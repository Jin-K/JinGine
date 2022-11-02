﻿using System.Runtime.InteropServices;

namespace JinGine.WinForms.Views;

internal class Win32Caret
{
    private readonly UserControl _userControl;
    private Point _location;
    private bool _exists;

    internal int Width { get; set; }

    internal int Height { get; set; }

    internal Win32Caret(UserControl userControl)
    {
        _userControl = userControl;
        _location = Point.Empty;
        _exists = false;

        userControl.GotFocus += OnGotFocus;
        userControl.LostFocus += OnLostFocus;
        userControl.Paint += OnPaint;
    }

    internal void SetLocation(Point location, bool force = false)
    {
        //if (location != _location)
        //{
        //    Debug.WriteLine(nameof(SetLocation));
        //    Debug.Indent();
        //    Debug.WriteLine($"Old Value: {_location}");
        //    Debug.WriteLine($"New Value: {location}");
        //    Debug.Unindent();
        //}
        _location = location;
        if (force is false) return;

        if (_exists is false)
        {
            CreateCaret();
            SetCaretPos();
            ShowCaret();
        }
        else
        {
            SetCaretPos();
        }

        _userControl.Focus();
    }

    private void CreateCaret()
    {
        if (!Succeeded(CreateCaret(_userControl.Handle, IntPtr.Zero, Width, Height)))
            throw new COMException(nameof(CreateCaret), Marshal.GetLastWin32Error());
        _exists = true;
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
            throw new COMException(nameof(DestroyCaret), Marshal.GetLastWin32Error());
        _exists = false;
    }

    private void OnPaint(object? sender, PaintEventArgs e)
    {
        if (_exists is false) return;
        SetCaretPos();
    }

    private void SetCaretPos()
    {
        if (!Succeeded(SetCaretPos(_location.X, _location.Y)))
            throw new COMException(nameof(SetCaretPos), Marshal.GetLastWin32Error());
    }

    private void ShowCaret()
    {
        if (!Succeeded(ShowCaret(_userControl.Handle)))
            throw new COMException(nameof(ShowCaret), Marshal.GetLastWin32Error());
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