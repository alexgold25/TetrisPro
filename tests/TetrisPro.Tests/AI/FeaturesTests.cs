using TetrisPro.Core.AI;
using TetrisPro.Core.Models;
using Xunit;

namespace TetrisPro.Tests.AI;

public class FeaturesTests
{
    [Fact]
    public void ExtractsHolesBumpinessAndWells()
    {
        var b = new Board();
        // Column 0: two blocks with a hole between
        b[0, Board.TotalHeight - 1] = PieceType.I;
        b[0, Board.TotalHeight - 3] = PieceType.I;
        // Column 1: one block
        b[1, Board.TotalHeight - 1] = PieceType.I;

        var res = Features.Extract(b);
        Assert.Equal(4, res.AggregateHeight); // heights: 3 + 1
        Assert.Equal(1, res.Holes);
        Assert.Equal(3, res.Bumpiness); // |3-1| + |1-0|
        Assert.Equal(0, res.WellSum);
    }
}
