using System.Collections.Generic;
using TetrisPro.Core.Services;

namespace TetrisPro.App.Services;

public class InputService : IInputService
{
    private readonly HashSet<InputKey> _pressed = new();

    public bool IsDown(InputKey key) => _pressed.Contains(key);

    public void KeyDown(InputKey key) => _pressed.Add(key);

    public void KeyUp(InputKey key) => _pressed.Remove(key);
}
