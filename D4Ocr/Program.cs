using D4Ocr;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Diablo 4 OCR");

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("config.json", false);

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
using var engine = TesseractEngineFactory.Create();

var parser = new OcrParser(engine, godRolls);

Console.WriteLine("Locating Diablo 4 process");
var handle = ProcessLocator.MainHandle("Diablo IV");

var desktopResolution = Window.Resolution(handle);
Console.WriteLine("Desktop resolution {0}x{1}", desktopResolution.Width, desktopResolution.Height);
Console.WriteLine("Game resolution {0}x{1}", gameResolution.Width, gameResolution.Height);

var scale = new ResolutionScale(desktopResolution, gameResolution);

var appState = new ApplicationState();
var captureMethod = new GraphicsCopyFromScreenMethod(gameResolution);
var tooltipProvider = new TooltipProvider(appState, captureMethod, parser);

Console.WriteLine("Running overlay");
using var overlay = new Overlay(scale, handle, appState);
overlay.Run();

Console.WriteLine("Running screen capturing");
var cts = new CancellationTokenSource();
tooltipProvider.Run(cts.Token);

Console.WriteLine("Game time!");
Console.WriteLine("Press D to store next 10 captures into debug folder.");
Console.WriteLine("Press Q to quit OCR...");
while (!cts.Token.IsCancellationRequested)
{
    var key = Console.ReadKey(true);
    if (key.Key == ConsoleKey.Q)
    {
        cts.Cancel();
    }

    if (key.Key == ConsoleKey.D)
    {
        appState.Debug = true;
    }
}