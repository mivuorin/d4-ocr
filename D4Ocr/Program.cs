using System.Collections.Concurrent;
using D4Ocr;
using Microsoft.Extensions.Configuration;
using Tesseract;

Console.WriteLine("Diablo 4 OCR");

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("config.json", optional: false);

IConfiguration config = builder.Build();
var gameResolution = config.GetRequiredSection("resolution").Get<Resolution>();
if (gameResolution == null)
{
    throw new InvalidOperationException("Cannot read resolution configuration");
}

var godRolls = config.GetRequiredSection("affixes").Get<Dictionary<string, string[]>>();
if (godRolls is null)
{
    throw new InvalidOperationException(@"Cannot read affixes configuration");
}

Console.WriteLine("Initializing OCR Engine");
using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);

var parser = new OcrParser(engine, godRolls);

Console.WriteLine("Locating Diablo 4 process");
var handle = ProcessLocator.MainHandle();

var desktopResolution = Window.Resolution(handle);
Console.WriteLine("Desktop screen resolution {0}x{1}", desktopResolution.Width, desktopResolution.Height);
Console.WriteLine("Starting overlay for game resolution {0}x{1}", gameResolution.Width, gameResolution.Height);

var scale = new ResolutionScale(desktopResolution, gameResolution);

var rectangles = new ConcurrentBag<GameOverlay.Drawing.Rectangle>();
using var overlay = new Overlay(scale, handle, rectangles);
overlay.Run();

Console.WriteLine("Running OCR");
var cts = new CancellationTokenSource();
var debug = false;

ThreadPool.QueueUserWorkItem(CaptureTooltips, cts.Token);

Console.WriteLine("Game time! Press Q to quit OCR...");
while (!cts.Token.IsCancellationRequested)
{
    var key = Console.ReadKey(true);
    if (key.Key == ConsoleKey.Q)
    {
        cts.Cancel();
    }

    if (key.Key == ConsoleKey.D)
    {
        debug = true;
    }
}

void CaptureTooltips(object? state)
{
    if (state is null)
    {
        throw new ArgumentNullException(nameof(state));
    }

    var token = (CancellationToken)state;

    var debugImageCount = 0;
    
    while (!token.IsCancellationRequested)
    {
        using var bitmap = ScreenCapture.Capture(handle);

        if (debug)
        {
            bitmap.Save($@"c:\temp\debug-{debugImageCount++}.png");
            Console.WriteLine("Debug screenshot saved!");

            if (debugImageCount > 10)
            {
                debug = false;
                debugImageCount = 0;
                Console.WriteLine("Debug stopped.");
            }
        }

        var identified = parser.Identify(bitmap);

        rectangles.Clear();
        foreach (var r in identified)
        {
            rectangles.Add(r);
        }
    }
}