using System.Threading;
using TetrisPro.Core.Models;

namespace TetrisPro.Core.AI;

/// <summary>High level AI agent that ties together search, evaluation and input planning.</summary>
public class AiAgent : IAiAgent
{
    private readonly BeamSearch _search;
    private readonly HumanInputEmulator _emulator;
    private readonly EvalWeights _weights;

    public AiAgent(EvalWeights? weights = null, int ply = 1, int beamWidth = 32)
    {
        _weights = weights ?? EvalWeights.Default;
        _search = new BeamSearch(ply, beamWidth);
        _emulator = new HumanInputEmulator();
    }

    public AiDecision DecideNextMove(GameState state, CancellationToken ct = default)
    {
        var (placement, score) = _search.Search(state, _weights);
        if (state.ActivePiece == null)
            return new AiDecision { Target = placement, EvalScore = score };
        var plan = _emulator.BuildPlan(state.ActivePiece, placement);
        return new AiDecision { Target = placement, InputPlan = plan, EvalScore = score };
    }
}
