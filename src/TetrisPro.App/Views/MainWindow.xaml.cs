using System;
using System.Windows;
using System.Windows.Input;
using TetrisPro.Core.Engine;
using TetrisPro.Core.Services;
using TetrisPro.App.ViewModels;
using TetrisPro.App.Services;

namespace TetrisPro.App.Views;

public partial class MainWindow : Window
{
    private readonly IInputService _input;
    private readonly RenderTicker _ticker;
    private readonly IGameEngine _engine;

    public MainWindow(IInputService input, RenderTicker ticker, IGameEngine engine, MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        _input = input;
        _ticker = ticker;
        _engine = engine;
        _ticker.Tick += OnTick;
        _ticker.Start();
        KeyDown += OnKeyDownHandler;
        KeyUp += OnKeyUpHandler;
    }

    private void OnTick(object? sender, TimeSpan delta)
    {
        _engine.Update(delta);
        if (DataContext is MainViewModel vm)
            vm.UpdateState();
    }

    private void OnKeyDownHandler(object sender, KeyEventArgs e)
    {
        if (TryMap(e.Key, out var key))
            _input.KeyDown(key);
    }

    private void OnKeyUpHandler(object sender, KeyEventArgs e)
    {
        if (TryMap(e.Key, out var key))
            _input.KeyUp(key);
    }

    private static bool TryMap(Key key, out InputKey result)
    {
        switch (key)
        {
            case Key.Left: result = InputKey.Left; return true;
            case Key.Right: result = InputKey.Right; return true;
            case Key.Up:
            case Key.X: result = InputKey.RotateCW; return true;
            case Key.Z: result = InputKey.RotateCCW; return true;
            case Key.A: result = InputKey.Rotate180; return true;
            case Key.Down: result = InputKey.SoftDrop; return true;
            case Key.Space: result = InputKey.HardDrop; return true;
            case Key.LeftShift:
            case Key.RightShift:
            case Key.C: result = InputKey.Hold; return true;
            case Key.P: result = InputKey.Pause; return true;
            default: result = default; return false;
        }
    }
}
