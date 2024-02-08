using System.Collections.Concurrent;
using GameOverlay.Drawing;

namespace D4Ocr;

public class ApplicationState
{
    private readonly object _lock = new();
    private bool _debug;

    public ApplicationState()
    {
        Tooltips = new ConcurrentBag<Rectangle>();
        Debug = false;
        MeasuredMs = 0;
    }

    public ConcurrentBag<Rectangle> Tooltips { get; }

    public bool Debug
    {
        get
        {
            lock (_lock)
            {
                return _debug;
            }
        }

        set
        {
            lock (_lock)
            {
                _debug = value;
            }
        }
    }

    public long MeasuredMs { get; set; }
}