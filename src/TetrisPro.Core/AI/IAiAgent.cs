using System.Threading;
using TetrisPro.Core.Models;

namespace TetrisPro.Core.AI;

/// <summary>Public entry point for the Tetris AI agent.</summary>
public interface IAiAgent
{
    /// <summary>
    /// Calculates the best move for the given game state and returns a plan of inputs
    /// that execute it in a human-like manner.
    /// </summary>
    AiDecision DecideNextMove(GameState state, CancellationToken ct = default);
}
