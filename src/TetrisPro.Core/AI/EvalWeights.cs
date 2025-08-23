namespace TetrisPro.Core.AI;

/// <summary>Weights for the heuristic evaluator.</summary>
public readonly record struct EvalWeights(
    double WLineClear,
    double WAggregateHeight,
    double WHoles,
    double WBumpiness,
    double WWellSum,
    double WOverhang,
    double WFinessePenalty,
    double WTSpin,
    double WCombo,
    double WBTB,
    double WPerfectClear)
{
    public static EvalWeights Default => new(
        1.0, -0.90, -0.80, -0.40, -0.60, -0.25, -0.10, 1.20, 0.25, 0.35, 3.0);
}
