﻿namespace D4Ocr;

using System.Drawing;
using PInvoke;

public static class ScreenCapture
{
    const int SRCCOPY = 0x00CC0020;
    const int CAPTUREBLT = 0x40000000;

    public static Bitmap Capture(IntPtr source)
    {
        IntPtr bitmap = 0;
        IntPtr oldBitmap = 0;

        var desktopHandle = User32.GetDesktopWindow();
        var desktopDc = User32.GetWindowDC(desktopHandle);
        var memoryDc = Gdi32.CreateCompatibleDC(desktopDc);

        try
        {
            var bounds = Window.Resolution(source);
            
            bitmap = Gdi32.CreateCompatibleBitmap(desktopDc, bounds.Width, bounds.Height);
            oldBitmap = Gdi32.SelectObject(memoryDc, bitmap);

            if (!Gdi32.BitBlt(memoryDc, 0, 0, bounds.Width, bounds.Height,
                    desktopDc, 0, 0, SRCCOPY | CAPTUREBLT))
            {
                throw new InvalidOperationException("Gdi32.BitBlt failed");
            }

            return Image.FromHbitmap(bitmap);
        }
        finally
        {
            Gdi32.SelectObject(memoryDc, oldBitmap);
            Gdi32.DeleteObject(bitmap);
            Gdi32.DeleteDC(memoryDc);
            User32.ReleaseDC(desktopHandle, desktopDc.DangerousGetHandle());
        }
    }
}