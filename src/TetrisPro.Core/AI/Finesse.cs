using TetrisPro.Core.Models;

namespace TetrisPro.Core.AI;

/// <summary>Provides crude estimation of input cost to reach a placement.</summary>
public static class Finesse
{
    /// <summary>Estimate number of key presses needed to move the piece to target.</summary>
    public static int EstimateCost(Tetromino piece, Placement target)
    {
        int dx = System.Math.Abs(target.X - piece.Position.X);
        int rotDiff = (target.Rotation - (int)piece.Rotation) & 3;
        int rotCost = rotDiff switch { 0 => 0, 1 => 1, 2 => 1, 3 => 1, _ => 0 };
        return dx + rotCost;
    }
}
