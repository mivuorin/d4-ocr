namespace D4Ocr;

public class TooltipProvider
{
    private readonly ApplicationState _appState;
    private readonly ICaptureMethod _captureMethod;
    private readonly OcrParser _ocrParser;

    public TooltipProvider(ApplicationState appState, ICaptureMethod captureMethod, OcrParser ocrParser)
    {
        _appState = appState;
        _captureMethod = captureMethod;
        _ocrParser = ocrParser;
    }

    public void Run(CancellationToken token)
    {
        ThreadPool.QueueUserWorkItem(CaptureTooltips, token);
    }

    private void CaptureTooltips(object? state)
    {
        if (state is null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        var token = (CancellationToken)state;

        var debugImageCount = 0;

        while (!token.IsCancellationRequested)
        {
            using var bitmap = _captureMethod.Capture();

            if (_appState.Debug)
            {
                bitmap.Save($@"c:\temp\debug-{debugImageCount++}.png");
                Console.WriteLine("Debug screenshot saved!");
            
                if (debugImageCount > 10)
                {
                    _appState.Debug = false;
                    debugImageCount = 0;
                    Console.WriteLine("Debug stopped.");
                }
            }

            var identified = _ocrParser.Identify(bitmap);

            _appState.Tooltips.Clear();
            foreach (var r in identified)
            {
                _appState.Tooltips.Add(r);
            }
        }
    }
}