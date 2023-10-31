using System.Drawing;

namespace D4Ocr;

public class GraphicsCopyFromScreenMethod : ICaptureMethod
{
    private readonly Rectangle _bounds;

    public GraphicsCopyFromScreenMethod(Resolution resolution)
    {
        _bounds = new Rectangle(0, 0, resolution.Width, resolution.Height);
    }

    public Bitmap Capture()
    {
        var bitmap = new Bitmap(_bounds.Width, _bounds.Height);

        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(new Point(_bounds.Left, _bounds.Top), Point.Empty, _bounds.Size);

        return bitmap;
    }
}