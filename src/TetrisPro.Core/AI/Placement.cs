namespace TetrisPro.Core.AI;

/// <summary>Final position for a tetromino.</summary>
public readonly record struct Placement(int X, int Rotation, bool HardDrop);
