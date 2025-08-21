using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TetrisPro.Core.Config;
using TetrisPro.Core.Engine;
using TetrisPro.Core.Models;
using TetrisPro.Core.Services;
using Xunit;

namespace TetrisPro.Tests.Core;

public class TopOutTests
{
    [Fact]
    public void TopOutOccursWhenSpawnBlocked()
    {
        var input = new TestInput();
        var engine = new GameEngine(new SequenceRandomizer(new[]{PieceType.I, PieceType.I}), input, new SystemTimeProvider(), NullLogger<GameEngine>.Instance);
        engine.StartNewGame(new GameConfig());
        // block spawn area for next piece
        engine.State.Board[4,0] = PieceType.O;
        // hard drop current piece
        input.KeyDown(InputKey.HardDrop);
        engine.Update(TimeSpan.Zero);
        engine.State.Status.Should().Be(GameStatus.TopOut);
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
