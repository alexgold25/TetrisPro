using System;
using System.Windows.Media;

namespace TetrisPro.App.Services;

public class RenderTicker
{
    private DateTime _last;
    public event EventHandler<TimeSpan>? Tick;

    public void Start()
    {
        _last = DateTime.Now;
        CompositionTarget.Rendering += OnRendering;
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        var now = DateTime.Now;
        var delta = now - _last;
        _last = now;
        Tick?.Invoke(this, delta);
    }
}
