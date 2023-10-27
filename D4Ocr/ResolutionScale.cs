namespace D4Ocr;

using GameOverlay.Drawing;

public class ResolutionScale
{
    private readonly float _xScale;
    private readonly float _yScale;

    public ResolutionScale(Resolution desktop, Resolution game)
    {
        _xScale = (float)desktop.Width / game.Width;
        _yScale = (float)desktop.Height / game.Height;
    }

    public Rectangle Scale(Rectangle rectangle)
    {
        return Rectangle.Create(rectangle.Left * _xScale, rectangle.Top * _yScale, rectangle.Width, rectangle.Height);
    }
}