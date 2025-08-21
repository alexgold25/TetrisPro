namespace TetrisPro.Core.Services;

public enum InputKey
{
    Left, Right, RotateCW, RotateCCW, Rotate180, SoftDrop, HardDrop, Hold, Pause
}

public interface IInputService
{
    bool IsDown(InputKey key);
    void KeyDown(InputKey key);
    void KeyUp(InputKey key);
}
