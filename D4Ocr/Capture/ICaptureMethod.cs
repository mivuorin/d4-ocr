using System.Drawing;

namespace D4Ocr.Capture;

public interface ICaptureMethod
{
    Bitmap Capture();
}