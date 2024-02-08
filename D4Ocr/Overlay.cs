using System.Text;
using GameOverlay.Drawing;
using GameOverlay.Windows;

namespace D4Ocr;

public class Overlay : IDisposable
{
    private readonly ApplicationState _appState;
    private readonly Dictionary<string, SolidBrush> _brushes;
    private readonly Dictionary<string, Font> _fonts;
    private readonly ResolutionScale _scale;
    private readonly GraphicsWindow _window;

    public Overlay(ResolutionScale scale, IntPtr parent,
        ApplicationState appState)
    {
        _scale = scale;
        _appState = appState;

        _brushes = new Dictionary<string, SolidBrush>();
        _fonts = new Dictionary<string, Font>();

        var gfx = new Graphics
        {
            MeasureFPS = true,
            PerPrimitiveAntiAliasing = true,
            TextAntiAliasing = true
        };

        _window = new StickyWindow(parent, gfx)
        {
            FPS = 30,
            IsTopmost = true,
            IsVisible = true
        };

        _window.SetupGraphics += OnSetupGraphics;
        _window.DrawGraphics += OnDrawGraphics;
        _window.DestroyGraphics += OnDestroyGraphics;
    }

    public void Dispose()
    {
        _window.Dispose();
        GC.SuppressFinalize(this);
    }

    public void Run()
    {
        _window.Create();
    }

    private void OnDestroyGraphics(object? sender, DestroyGraphicsEventArgs e)
    {
        foreach (var pair in _brushes)
        {
            pair.Value.Dispose();
        }

        foreach (var pair in _fonts)
        {
            pair.Value.Dispose();
        }
    }

    private void OnDrawGraphics(object? sender, DrawGraphicsEventArgs e)
    {
        var gfx = e.Graphics;

        gfx.ClearScene();

        var padding = 8;
        var infoText = new StringBuilder()
            .Append("FPS: ").Append(gfx.FPS.ToString().PadRight(padding))
            .Append("FrameTime: ").Append(e.FrameTime.ToString().PadRight(padding))
            .Append("FrameCount: ").Append(e.FrameCount.ToString().PadRight(padding))
            .Append("DeltaTime: ").Append(e.DeltaTime.ToString().PadRight(padding))
            .Append("Affixes: ").Append(_appState.Tooltips.Count.ToString().PadRight(padding))
            .ToString();

        gfx.DrawTextWithBackground(_fonts["consolas"], _brushes["green"], _brushes["black"], 58, 20, infoText);

        foreach (var tooltip in _appState.Tooltips)
        {
            var scaled = _scale.Scale(tooltip);

            // Make sure overlay icon does not obfuscate OCR by drawing over text
            gfx.FillCircle(_brushes["green"], scaled.Left - 4f, scaled.Top + 5, 8);
        }
    }

    private void OnSetupGraphics(object? sender, SetupGraphicsEventArgs e)
    {
        var gfx = e.Graphics;

        if (e.RecreateResources)
        {
            foreach (var pair in _brushes)
            {
                pair.Value.Dispose();
            }
        }

        _brushes["black"] = gfx.CreateSolidBrush(0, 0, 0);
        _brushes["white"] = gfx.CreateSolidBrush(255, 255, 255);
        _brushes["red"] = gfx.CreateSolidBrush(255, 0, 0);
        _brushes["green"] = gfx.CreateSolidBrush(0, 255, 0);
        _brushes["background"] = gfx.CreateSolidBrush(0x33, 0x36, 0x3F);

        if (e.RecreateResources)
        {
            return;
        }

        // Fonts do not need to be recreated
        _fonts["arial"] = gfx.CreateFont("Arial", 12);
        _fonts["consolas"] = gfx.CreateFont("Consolas", 14);
    }
}