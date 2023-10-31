using System.Drawing;

namespace D4Ocr;

public interface ICaptureMethod
{
    Bitmap Capture();
}