using FluentAssertions;
using TetrisPro.Core.Engine;
using TetrisPro.Core.Models;
using Xunit;

namespace TetrisPro.Tests.Core;

public class SrsRotationTests
{
    [Fact]
    public void KicksContainFiveTests()
    {
        SrsData.JLSTZKicks[(Rotation.Spawn, Rotation.Right)].Should().HaveCount(5);
        SrsData.IKicks[(Rotation.Spawn, Rotation.Right)].Should().HaveCount(5);
    }
}
