using System;
using System.Collections.Generic;
using System.Linq;
using TetrisPro.Core.Engine;
using TetrisPro.Core.Models;
using TetrisPro.Core.Services;

namespace TetrisPro.Core.AI;

/// <summary>
/// Simple AI player that finds a good placement for the active piece and
/// simulates key presses on the <see cref="IInputService"/>.
/// </summary>
public class AutoPlayer
{
    private readonly IGameEngine _engine;
    private readonly IInputService _input;

    private Tetromino? _lastPiece;
    private readonly Queue<InputKey> _actions = new();
    private readonly HashSet<InputKey> _release = new();
    private bool _softDrop;

    public bool Enabled { get; set; }
    public int SpeedMultiplier { get; private set; } = 1;

    public AutoPlayer(IGameEngine engine, IInputService input)
    {
        _engine = engine;
        _input = input;
    }

    /// <summary>Cycles speed multiplier: 1x → 2x → 4x → 8x → 1x.</summary>
    public void CycleSpeed()
    {
        SpeedMultiplier = SpeedMultiplier switch { 1 => 2, 2 => 4, 4 => 8, _ => 1 };
    }

    public void ResetSpeed() => SpeedMultiplier = 1;

    public void Update()
    {
        // Release keys from previous frame.
        foreach (var k in _release.ToList())
        {
            _input.KeyUp(k);
            _release.Remove(k);
        }

        if (!Enabled)
        {
            ReleaseAll();
            return;
        }

        var piece = _engine.State.ActivePiece;
        if (piece != _lastPiece)
        {
            _lastPiece = piece;
            _actions.Clear();
            _softDrop = false;
            if (piece != null)
            {
                var plan = FindBestPlacement(_engine.State.Board, piece.Clone());
                EnqueueMoves(piece, plan.targetX, plan.targetRot);
            }
        }

        if (_actions.Count > 0)
        {
            var key = _actions.Dequeue();
            _input.KeyDown(key);
            _release.Add(key);
        }
        else if (!_softDrop)
        {
            _input.KeyDown(InputKey.SoftDrop);
            _softDrop = true;
        }
    }

    private void ReleaseAll()
    {
        if (_softDrop)
        {
            _input.KeyUp(InputKey.SoftDrop);
            _softDrop = false;
        }
        foreach (var k in _release.ToList())
        {
            _input.KeyUp(k);
            _release.Remove(k);
        }
        _actions.Clear();
        _lastPiece = null;
    }

    private void EnqueueMoves(Tetromino current, int targetX, Rotation targetRot)
    {
        // Rotation moves
        int diff = ((int)targetRot - (int)current.Rotation + 4) & 3;
        switch (diff)
        {
            case 1: _actions.Enqueue(InputKey.RotateCW); break;
            case 2: _actions.Enqueue(InputKey.Rotate180); break;
            case 3: _actions.Enqueue(InputKey.RotateCCW); break;
        }

        // Horizontal moves
        int dx = targetX - current.Position.X;
        if (dx < 0)
            for (int i = 0; i < -dx; i++) _actions.Enqueue(InputKey.Left);
        else if (dx > 0)
            for (int i = 0; i < dx; i++) _actions.Enqueue(InputKey.Right);
    }

    private (int targetX, Rotation targetRot) FindBestPlacement(Board board, Tetromino piece)
    {
        double bestScore = double.NegativeInfinity;
        int bestX = piece.Position.X;
        Rotation bestRot = piece.Rotation;

        foreach (Rotation rot in Enum.GetValues(typeof(Rotation)))
        {
            var p = piece.Clone();
            p.Rotation = rot;
            // Adjust starting Y so that piece is fully inside board.
            int minY = p.Cells.Min(c => c.Y);
            int startY = -minY;
            p.Position = new PointI(p.Position.X, startY);

            int minX = -p.Cells.Min(c => c.X);
            int maxX = Board.Width - 1 - p.Cells.Max(c => c.X);
            for (int x = minX; x <= maxX; x++)
            {
                p.Position = new PointI(x, startY);
                var b = CloneBoard(board);
                // Drop piece
                while (!b.IsCollision(p))
                    p.Position += new PointI(0, 1);
                p.Position -= new PointI(0, 1);

                foreach (var c in p.Cells)
                {
                    var pos = c + p.Position;
                    b[pos.X, pos.Y] = p.Type;
                }
                int lines = b.ClearFullLines();
                double score = Evaluate(b, lines);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestX = x;
                    bestRot = rot;
                }
            }
        }
        return (bestX, bestRot);
    }

    private static Board CloneBoard(Board source)
    {
        var b = new Board();
        for (int x = 0; x < Board.Width; x++)
            for (int y = 0; y < Board.TotalHeight; y++)
                b[x, y] = source[x, y];
        return b;
    }

    private static double Evaluate(Board board, int lines)
    {
        int[] heights = new int[Board.Width];
        int holes = 0;
        for (int x = 0; x < Board.Width; x++)
        {
            bool block = false;
            for (int y = 0; y < Board.TotalHeight; y++)
            {
                if (board[x, y].HasValue)
                {
                    if (!block)
                    {
                        heights[x] = Board.TotalHeight - y;
                        block = true;
                    }
                }
                else if (block)
                {
                    holes++;
                }
            }
        }

        int aggregateHeight = heights.Sum();
        int bumpiness = 0;
        for (int x = 0; x < Board.Width - 1; x++)
            bumpiness += Math.Abs(heights[x] - heights[x + 1]);

        return 0.76 * lines - 0.18 * aggregateHeight - 0.51 * holes - 0.36 * bumpiness;
    }
}

