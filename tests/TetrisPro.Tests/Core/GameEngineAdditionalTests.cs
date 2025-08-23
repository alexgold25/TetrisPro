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

public class GameEngineAdditionalTests
{
    [Fact]
    public void StartNewGameInitializesNextQueue()
    {
        var input = new TestInput();
        var seq = new[]
        {
            PieceType.I, PieceType.J, PieceType.L, PieceType.O, PieceType.S, PieceType.T, PieceType.Z
        };
        var engine = new GameEngine(new SequenceRandomizer(seq), input, new SystemTimeProvider(), NullLogger<GameEngine>.Instance);
        engine.StartNewGame(new GameConfig());
        engine.State.NextQueue.Should().HaveCount(5);
    }

    [Fact]
    public void ElapsedTimePausesAndResumes()
    {
        var input = new TestInput();
        var seq = new[]
        {
            PieceType.I, PieceType.J, PieceType.L, PieceType.O, PieceType.S, PieceType.T, PieceType.Z
        };
        var engine = new GameEngine(new SequenceRandomizer(seq), input, new SystemTimeProvider(), NullLogger<GameEngine>.Instance);
        engine.StartNewGame(new GameConfig());

        engine.Update(TimeSpan.FromSeconds(1));
        engine.State.Elapsed.Should().Be(TimeSpan.FromSeconds(1));

        engine.Pause();
        engine.Update(TimeSpan.FromSeconds(1));
        engine.State.Elapsed.Should().Be(TimeSpan.FromSeconds(1));

        engine.Resume();
        engine.Update(TimeSpan.FromSeconds(1));
        engine.State.Elapsed.Should().Be(TimeSpan.FromSeconds(2));
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

