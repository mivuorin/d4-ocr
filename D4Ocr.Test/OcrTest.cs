using System.Reflection;
using GameOverlay.Drawing;
using Tesseract;

namespace D4Ocr.Test;

public class OcrTest
{
    private TesseractEngine _engine = null!;
    private OcrParser _ocrParser = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);

        var godRolls = new Dictionary<string, string[]>()
        {
            { "weapon", new[] { "dexterity", "all stats" } },
            { "helm", new[] { "resistance", "cold imbuement" } },
            { "amulet", new[] { "damage", "ranks of all imbuement", "lucky hit chance" } },
            { "ring", new[] { "critical strike chance", "maximum life", "lucky hit chance" } },
            { "boots", new[] { "willpower", "slow duration", "energy cost reduction" } },
            { "pants", new[] { "shadow resistance", "dexterity", "all stats" } },
            {
                "gloves",
                new[] { "critical strike chance", "critical strike damage", "trap skill arm time", "rank of rapid" }
            },
            { "chest", new[] { "total armor", "maximum life", "damage reduction from close", "distant enemies" } },
        };

        _ocrParser = new OcrParser(_engine, godRolls);
    }

    private System.Drawing.Bitmap Resource(string imageName)
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"D4Ocr.Test.{imageName}.png");
        return new System.Drawing.Bitmap(stream!);
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        _engine.Dispose();
    }

    [Test]
    public void Identify_weapon_dexterity_affix()
    {
        using var bitmap = Resource("weapon-dagger");
        var rectangles = _ocrParser.Identify(bitmap);

        var expected = new Rectangle
        {
            Left = 60, Top = 493, Right = 592, Bottom = 524
        };

        Assert.That(rectangles.First(), Is.EqualTo(expected));
    }

    [Test]
    public void Identify_weapon_all_stats_affix()
    {
        using var bitmap = Resource("weapon-bow");
        var rectangles = _ocrParser.Identify(bitmap);

        // Comparison matches lost dexterity property
        Assert.That(rectangles.Count(), Is.EqualTo(2));
    }

    [Test]
    public void Identify_helmet_affixes()
    {
        using var bitmap = Resource("helmet");
        var rectangles = _ocrParser.Identify(bitmap);

        Assert.That(rectangles.Count(), Is.EqualTo(2));
    }

    [Test]
    public void Identify_amulet_affixes()
    {
        using var bitmap = Resource("amulet");
        var rectangles = _ocrParser.Identify(bitmap);

        // TODO Damage is found multiple times.
        Assert.That(rectangles.Count(), Is.EqualTo(4));
    }

    [Test]
    public void Identify_ring_affixes()
    {
        using var bitmap = Resource("ring");
        var rectangles = _ocrParser.Identify(bitmap);

        Assert.That(rectangles.Count(), Is.EqualTo(3));
    }

    [Test]
    public void Identify_boots_affixes()
    {
        using var bitmap = Resource("boots");
        var rectangles = _ocrParser.Identify(bitmap);

        Assert.That(rectangles.Count(), Is.EqualTo(3));
    }

    [Test]
    public void Identify_pants_affixes()
    {
        using var bitmap = Resource("pants");
        var rectangles = _ocrParser.Identify(bitmap);

        Assert.That(rectangles.Count(), Is.EqualTo(3));
    }

    [Test]
    public void Identify_armor_affixes()
    {
        using var bitmap = Resource("armor");
        var rectangles = _ocrParser.Identify(bitmap);

        // Maximum life exist 3 times
        Assert.That(rectangles.Count(), Is.EqualTo(6));
    }

    [Test]
    public void Identify_gloves_affixes()
    {
        using var bitmap = Resource("gloves");
        var rectangles = _ocrParser.Identify(bitmap);

        Assert.That(rectangles.Count(), Is.EqualTo(4));
    }
}