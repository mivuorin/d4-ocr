using System.Drawing;
using System.Drawing.Imaging;

namespace D4Ocr.Capture;

public class CaptureDebugger : ICaptureMethod
{
    private readonly ICaptureMethod _captureMethod;
    private readonly ApplicationState _appState;
    private int _imageCount;

    public CaptureDebugger(ICaptureMethod captureMethod, ApplicationState appState)
    {
        _captureMethod = captureMethod;
        _appState = appState;
        _imageCount = 0;
    }

    public Bitmap Capture()
    {
        var bitmap = _captureMethod.Capture();

        if (_appState.Debug)
        {
            var dir = Path.Join(Environment.CurrentDirectory, "debug");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            var filename = Path.Join(dir, $@"debug-{_imageCount++}.png" );
            bitmap.Save(filename, ImageFormat.Png );
            Console.WriteLine("Debug screenshot saved!");

            if (_imageCount > 10)
            {
                _appState.Debug = false;
                _imageCount = 0;
                Console.WriteLine("Debug stopped.");
            }
        }

        return bitmap;
    }
}