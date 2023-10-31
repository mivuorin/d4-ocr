using System.Drawing;
using Tesseract;
using Rectangle = GameOverlay.Drawing.Rectangle;

namespace D4Ocr;

public class OcrParser
{
    private static readonly Dictionary<string, string> ItemTypes = new()
    {
        { "helm", "helm" },
        { "weapon", "Damage per second" },
        { "amulet", "amulet" },
        { "ring", "ring" },
        { "boots", "boots" },
        { "pants", "pants" },
        { "gloves", "gloves" },
        { "chest", "chest" }
    };

    private readonly TesseractEngine _engine;
    private readonly Dictionary<string, string[]> _godRolls;

    public OcrParser(TesseractEngine engine, Dictionary<string, string[]> godRolls)
    {
        _engine = engine;
        _godRolls = godRolls;
    }

    public IEnumerable<Rectangle> Identify(Bitmap bitmap)
    {
        using var pix = PixConverter.ToPix(bitmap);
        using var page = _engine.Process(pix);
        using var iter = page.GetIterator();

        iter.Begin();

        var identifiedAffixes = new List<Rectangle>();
        string? itemType = null;

        while (iter.Next(PageIteratorLevel.TextLine))
        {
            var text = iter.GetText(PageIteratorLevel.TextLine);

            // Identify item
            if (itemType is null)
            {
                foreach (var keyValuePair in ItemTypes)
                {
                    if (text.Contains(keyValuePair.Value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        itemType = keyValuePair.Key;
                        break;
                    }
                }
            }
            else
            {
                var affixes = _godRolls[itemType];

                foreach (var affix in affixes)
                {
                    if (text.Contains(affix, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (iter.TryGetBoundingBox(PageIteratorLevel.TextLine, out var box))
                        {
                            var rectangle = Rectangle.Create(box.X1, box.Y1, box.Width, box.Height);
                            identifiedAffixes.Add(rectangle);
                        }

                        break;
                    }
                }
            }
        }

        return identifiedAffixes;
    }
}