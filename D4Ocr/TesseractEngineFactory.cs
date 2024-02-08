using Tesseract;

namespace D4Ocr;

public static class TesseractEngineFactory
{
    public static TesseractEngine Create()
    {
        return new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
    }
}