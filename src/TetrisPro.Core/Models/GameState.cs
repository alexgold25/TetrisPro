using System;
using System.Collections.Generic;

namespace TetrisPro.Core.Models;

/// <summary>Snapshot of current game state exposed to UI.</summary>
public class GameState
{
    public Board Board { get; set; } = new();
    public Tetromino? ActivePiece { get; set; }
    public Tetromino? GhostPiece { get; set; }
    public Tetromino? HoldPiece { get; set; }
    public IReadOnlyList<PieceType> NextQueue { get; set; } = new List<PieceType>();
    public int Score { get; set; }
    public int Level { get; set; }
    public int Lines { get; set; }
    /// <summary>Total time the current game has been active.</summary>
    public TimeSpan Elapsed { get; set; }
    public GameStatus Status { get; set; } = GameStatus.Paused;
}
