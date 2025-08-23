using System.Collections.Generic;
using TetrisPro.Core.Models;
using TetrisPro.Core.Services;

namespace TetrisPro.Core.AI;

/// <summary>Builds a list of input events that perform a placement with DAS/ARR timings.</summary>
public class HumanInputEmulator
{
    private readonly int _dasMs;
    private readonly int _arrMs;
    private readonly int _tapMs;
    private readonly int _jitterMs;

    public HumanInputEmulator(int dasMs = 120, int arrMs = 10, int tapMs = 35, int jitterMs = 15)
    {
        _dasMs = dasMs;
        _arrMs = arrMs;
        _tapMs = tapMs;
        _jitterMs = jitterMs;
    }

    /// <summary>Generates a simple input plan for moving <paramref name="piece"/> to <paramref name="target"/>.</summary>
    public IReadOnlyList<InputEvent> BuildPlan(Tetromino piece, Placement target)
    {
        var result = new List<InputEvent>();
        int t = 0;

        int dx = target.X - piece.Position.X;
        InputKey moveKey = dx < 0 ? InputKey.Left : InputKey.Right;
        for (int i = 0; i < System.Math.Abs(dx); i++)
        {
            result.Add(new InputEvent(moveKey, t));
            t += _tapMs + _jitterMs;
        }

        int rotDiff = (target.Rotation - (int)piece.Rotation) & 3;
        InputKey rotKey = InputKey.RotateCW;
        if (rotDiff == 3) rotKey = InputKey.RotateCCW;
        if (rotDiff == 2) rotKey = InputKey.Rotate180;
        for (int i = 0; i < rotDiff && rotDiff != 3; i++)
        {
            result.Add(new InputEvent(rotKey, t));
            t += _tapMs + _jitterMs;
        }
        if (rotDiff == 3)
        {
            result.Add(new InputEvent(rotKey, t));
            t += _tapMs + _jitterMs;
        }

        if (target.HardDrop)
            result.Add(new InputEvent(InputKey.HardDrop, t));

        return result;
    }
}
