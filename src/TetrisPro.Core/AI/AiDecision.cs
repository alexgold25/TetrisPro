using System.Collections.Generic;
using TetrisPro.Core.Models;

namespace TetrisPro.Core.AI;

/// <summary>Represents the result of an AI search.</summary>
public sealed class AiDecision
{
    /// <summary>Target placement for the current piece.</summary>
    public Placement Target { get; init; }

    /// <summary>Plan of key presses with timing information.</summary>
    public IReadOnlyList<InputEvent> InputPlan { get; init; } = new List<InputEvent>();

    /// <summary>Evaluation score of the chosen branch.</summary>
    public double EvalScore { get; init; }
}
