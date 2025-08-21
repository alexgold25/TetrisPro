using System;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TetrisPro.Core.Config;
using TetrisPro.Core.Engine;
using TetrisPro.Core.Models;
using TetrisPro.Core.Services;
using Xunit;

namespace TetrisPro.Tests.Core;

public class LockDelayTests
{
    [Fact]
    public void PieceDoesNotLockImmediately()
    {
        var engine = new GameEngine(new FixedRandomizer(PieceType.O), new DummyInput(), new SystemTimeProvider(), NullLogger<GameEngine>.Instance);
        engine.StartNewGame(new GameConfig { LockDelay = TimeSpan.FromMilliseconds(500) });
        // drop piece until it collides
        for (int i = 0; i < Board.VisibleHeight; i++)
            engine.Update(TimeSpan.FromSeconds(1));
        engine.State.ActivePiece.Should().NotBeNull();
        engine.Update(TimeSpan.FromMilliseconds(100));
        engine.State.ActivePiece.Should().NotBeNull();
        engine.Update(TimeSpan.FromMilliseconds(500));
        engine.State.ActivePiece.Should().BeNull();
    }

    private class FixedRandomizer : IRandomizer
    {
        private readonly PieceType _type;
        public FixedRandomizer(PieceType type) => _type = type;
        public PieceType Next() => _type;
    }

    private class DummyInput : IInputService
    {
        public bool IsDown(InputKey key) => false;
        public void KeyDown(InputKey key) { }
        public void KeyUp(InputKey key) { }
    }
}
