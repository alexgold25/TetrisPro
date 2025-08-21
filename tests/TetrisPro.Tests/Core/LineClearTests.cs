using FluentAssertions;
using TetrisPro.Core.Models;
using Xunit;

namespace TetrisPro.Tests.Core;

public class LineClearTests
{
    [Fact]
    public void FilledLineClears()
    {
        var board = new Board();
        for (int x = 0; x < Board.Width; x++)
            board[x, Board.TotalHeight-1] = PieceType.I;
        var cleared = board.ClearFullLines();
        cleared.Should().Be(1);
        for (int x = 0; x < Board.Width; x++)
            board[x, Board.TotalHeight-1].Should().BeNull();
    }
}
