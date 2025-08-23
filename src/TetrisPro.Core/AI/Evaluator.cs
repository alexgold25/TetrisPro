using TetrisPro.Core.Models;

namespace TetrisPro.Core.AI;

/// <summary>Simple heuristic evaluator based on board features.</summary>
public static class Evaluator
{
    /// <summary>Scores a placement by extracting features from the resulting board.</summary>
    public static double Score(Board before, Placement placement, Board after, EvalWeights w, int linesCleared, int finessePenalty = 0)
    {
        var feats = Features.Extract(after);
        double score = 0;
        score += w.WLineClear * linesCleared;
        score += w.WAggregateHeight * feats.AggregateHeight;
        score += w.WHoles * feats.Holes;
        score += w.WBumpiness * feats.Bumpiness;
        score += w.WWellSum * feats.WellSum;
        score += w.WFinessePenalty * finessePenalty;
        return score;
    }
}
