using PInvoke;

namespace D4Ocr;

public static class Window
{
    public static Resolution Resolution(IntPtr source)
    {
        if (!User32.GetWindowRect(source, out var rect))
        {
            throw new InvalidOperationException("User32.GetWindowRect failed");
        }

        return new Resolution { Width = rect.right, Height = rect.bottom };
    }
}