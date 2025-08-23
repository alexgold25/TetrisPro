using TetrisPro.Core.AI;
using TetrisPro.Core.Models;
using Xunit;

namespace TetrisPro.Tests.AI;

public class EvaluatorTests
{
    [Fact]
    public void PrefersBoardsWithFewerHolesAndLowerBumpiness()
    {
        var weights = EvalWeights.Default;
        var before = new Board();

        var better = new Board();
        better[0, Board.TotalHeight - 1] = PieceType.I;
        better[1, Board.TotalHeight - 1] = PieceType.I;

        var worse = new Board();
        worse[0, Board.TotalHeight - 1] = PieceType.I;
        worse[0, Board.TotalHeight - 3] = PieceType.I; // creates hole and height difference
        worse[1, Board.TotalHeight - 1] = PieceType.I;

        var placement = new Placement(0, 0, true);
        double betterScore = Evaluator.Score(before, placement, better, weights, 0);
        double worseScore = Evaluator.Score(before, placement, worse, weights, 0);

        Assert.True(betterScore > worseScore);
    }
}
