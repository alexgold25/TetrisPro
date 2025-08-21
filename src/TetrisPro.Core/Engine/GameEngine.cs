using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TetrisPro.Core.Config;
using TetrisPro.Core.Models;
using TetrisPro.Core.Services;

namespace TetrisPro.Core.Engine;

/// <summary>Main game engine handling state updates.</summary>
public class GameEngine : IGameEngine
{
    private readonly IRandomizer _randomizer;
    private readonly IInputService _input;
    private readonly ITimeProvider _time;
    private readonly ILogger<GameEngine> _logger;

    private GameConfig _config = new();
    private double _gravityCounter;
    private TimeSpan _lockCounter;
    private bool _holdUsed;

    public GameState State { get; private set; } = new();

    public GameEngine(IRandomizer randomizer, IInputService input, ITimeProvider time, ILogger<GameEngine> logger)
    {
        _randomizer = randomizer;
        _input = input;
        _time = time;
        _logger = logger;
    }

    public void StartNewGame(GameConfig config)
    {
        _config = config;
        State = new GameState
        {
            Board = new Board(),
            Score = 0,
            Level = 0,
            Lines = 0,
            Status = GameStatus.Spawning,
            NextQueue = new List<PieceType>()
        };
        _holdUsed = false;
        SpawnPiece();
    }

    public void Pause() => State.Status = GameStatus.Paused;
    public void Resume() => State.Status = GameStatus.Active;

    public void Update(TimeSpan delta)
    {
        if (State.Status != GameStatus.Active || State.ActivePiece is null)
            return;

        HandleInput();

        var piece = State.ActivePiece;
        bool wasGrounded = IsGrounded();

        var gravityPerSecond = _config.GravityByLevel[Math.Min(State.Level, _config.GravityByLevel.Count - 1)];
        _gravityCounter += delta.TotalSeconds * gravityPerSecond;
        while (_gravityCounter >= 1)
        {
            if (!TryMove(piece, new PointI(0,1)))
            {
                _gravityCounter = 0;
                break;
            }
            _gravityCounter -= 1;
        }

        var grounded = IsGrounded();
        if (grounded)
        {
            if (wasGrounded)
            {
                _lockCounter += delta;
                if (_lockCounter >= _config.LockDelay)
                    LockPiece();
            }
            else
            {
                _lockCounter = TimeSpan.Zero;
            }
        }
        else
        {
            _lockCounter = TimeSpan.Zero;
        }
    }

    private bool IsGrounded()
    {
        var test = State.ActivePiece!.Clone();
        test.Position += new PointI(0,1);
        return State.Board.IsCollision(test);
    }

    private void HandleInput()
    {
        var piece = State.ActivePiece!;
        if (_input.IsDown(InputKey.Left))
        {
            if (TryMove(piece, new PointI(-1,0))) _lockCounter = TimeSpan.Zero;
        }
        if (_input.IsDown(InputKey.Right))
        {
            if (TryMove(piece, new PointI(1,0))) _lockCounter = TimeSpan.Zero;
        }
        if (_input.IsDown(InputKey.RotateCW))
        {
            TryRotate(piece, true);
        }
        if (_input.IsDown(InputKey.RotateCCW))
        {
            TryRotate(piece, false);
        }
        if (_input.IsDown(InputKey.Rotate180))
        {
            piece.Rotate180();
            if (State.Board.IsCollision(piece)) piece.Rotate180();
        }
        if (_input.IsDown(InputKey.HardDrop))
        {
            int drop = 0;
            while (TryMove(piece, new PointI(0,1))) drop++;
            State.Score += Scoring.HardDrop(drop);
            LockPiece();
        }
        else if (_input.IsDown(InputKey.SoftDrop))
        {
            if (TryMove(piece, new PointI(0,1)))
            {
                State.Score += Scoring.SoftDrop(1);
            }
        }
        if (_input.IsDown(InputKey.Hold))
        {
            Hold();
        }
    }

    private bool TryMove(Tetromino piece, PointI delta, bool updateGhost = true)
    {
        piece.Position += delta;
        if (State.Board.IsCollision(piece))
        {
            piece.Position -= delta;
            return false;
        }
        if (updateGhost)
            UpdateGhost();
        return true;
    }

    private void TryRotate(Tetromino piece, bool cw)
    {
        var from = piece.Rotation;
        piece.Rotation = cw ? (Rotation)(((int)piece.Rotation + 1) & 3) : (Rotation)(((int)piece.Rotation + 3) & 3);
        var kicks = piece.Type == PieceType.I ? SrsData.IKicks[(from, piece.Rotation)] : SrsData.JLSTZKicks[(from, piece.Rotation)];
        foreach (var offset in kicks)
        {
            piece.Position += offset;
            if (!State.Board.IsCollision(piece))
            {
                _lockCounter = TimeSpan.Zero;
                UpdateGhost();
                return;
            }
            piece.Position -= offset;
        }
        piece.Rotation = from; // revert
    }

    private void Hold()
    {
        if (_holdUsed || State.ActivePiece is null) return;
        _holdUsed = true;
        var current = State.ActivePiece;
        var hold = State.HoldPiece;
        State.HoldPiece = current.Clone();
        State.HoldPiece.Position = new PointI(4,0);
        if (hold is null)
        {
            SpawnPiece();
        }
        else
        {
            State.ActivePiece = hold.Clone();
            State.ActivePiece.Position = new PointI(4,0);
            State.Status = GameStatus.Active;
            if (State.Board.IsCollision(State.ActivePiece))
                State.Status = GameStatus.TopOut;
            UpdateGhost();
        }
    }

    private void LockPiece()
    {
        var piece = State.ActivePiece!;
        foreach (var c in piece.Cells)
        {
            var p = c + piece.Position;
            if (State.Board.IsInside(p.X, p.Y))
                State.Board[p.X, p.Y] = piece.Type;
        }
        int cleared = State.Board.ClearFullLines();
        if (cleared > 0)
        {
            State.Score += Scoring.Lines(cleared);
            State.Lines += cleared;
            State.Level = State.Lines / 10;
        }
        _lockCounter = TimeSpan.Zero;
        _gravityCounter = 0;
        _holdUsed = false;
        SpawnPiece();
    }

    private void SpawnPiece()
    {
        var type = _randomizer.Next();
        var color = type switch
        {
            PieceType.I => "#00FFFF",
            PieceType.J => "#0000FF",
            PieceType.L => "#FFA500",
            PieceType.O => "#FFFF00",
            PieceType.S => "#00FF00",
            PieceType.T => "#800080",
            PieceType.Z => "#FF0000",
            _ => "#FFFFFF"
        };
        var piece = new Tetromino(type, color);
        if (State.Board.IsCollision(piece))
        {
            State.ActivePiece = null;
            State.Status = GameStatus.TopOut;
            return;
        }
        State.ActivePiece = piece;
        State.Status = GameStatus.Active;
        UpdateGhost();
    }

    private void UpdateGhost()
    {
        if (State.ActivePiece is null)
            return;
        var ghost = State.ActivePiece.Clone();
        while (TryMove(ghost, new PointI(0,1), updateGhost:false)) { }
        ghost.Position -= new PointI(0,1);
        State.GhostPiece = ghost;
    }
}
