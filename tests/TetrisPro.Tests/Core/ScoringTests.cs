using FluentAssertions;
using TetrisPro.Core.Engine;
using Xunit;

namespace TetrisPro.Tests.Core;

public class ScoringTests
{
    [Fact]
    public void LineScoresCorrectly()
    {
        Scoring.Lines(1).Should().Be(100);
        Scoring.Lines(4).Should().Be(800);
    }

    [Fact]
    public void DropScoresCorrectly()
    {
        Scoring.SoftDrop(2).Should().Be(2);
        Scoring.HardDrop(3).Should().Be(6);
    }
}
