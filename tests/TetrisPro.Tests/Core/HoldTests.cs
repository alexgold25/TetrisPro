using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TetrisPro.Core.Config;
using TetrisPro.Core.Engine;
using TetrisPro.Core.Models;
using TetrisPro.Core.Services;
using Xunit;

namespace TetrisPro.Tests.Core;

public class HoldTests
{
    [Fact]
    public void HoldSwapsCurrentPiece()
    {
        var input = new TestInput();
        var engine = new GameEngine(new SequenceRandomizer(new[]{PieceType.I, PieceType.J}), input, new SystemTimeProvider(), NullLogger<GameEngine>.Instance);
        engine.StartNewGame(new GameConfig());
        input.KeyDown(InputKey.Hold);
        engine.Update(TimeSpan.Zero);
        engine.State.HoldPiece.Should().NotBeNull();
        engine.State.ActivePiece!.Type.Should().Be(PieceType.J);
    }

    private class SequenceRandomizer : IRandomizer
    {
        private readonly Queue<PieceType> _seq;
        public SequenceRandomizer(IEnumerable<PieceType> seq) => _seq = new Queue<PieceType>(seq);
        public PieceType Next() => _seq.Dequeue();
    }

    private class TestInput : IInputService
    {
        private readonly HashSet<InputKey> _keys = new();
        public bool IsDown(InputKey key) => _keys.Contains(key);
        public void KeyDown(InputKey key) => _keys.Add(key);
        public void KeyUp(InputKey key) => _keys.Remove(key);
    }
}
