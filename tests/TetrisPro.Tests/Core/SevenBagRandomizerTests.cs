using System.Linq;
using FluentAssertions;
using TetrisPro.Core.Models;
using TetrisPro.Core.Services;
using Xunit;

namespace TetrisPro.Tests.Core;

public class SevenBagRandomizerTests
{
    [Fact]
    public void GeneratesUniqueSevenBag()
    {
        var rnd = new SevenBagRandomizer(0);
        var bag = Enumerable.Range(0,7).Select(_ => rnd.Next()).ToList();
        bag.Distinct().Should().HaveCount(7);
        var bag2 = Enumerable.Range(0,7).Select(_ => rnd.Next()).ToList();
        bag2.Distinct().Should().HaveCount(7);
    }
}
