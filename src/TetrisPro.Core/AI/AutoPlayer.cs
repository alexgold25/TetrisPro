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
    private readonly Queue<InputKey> _plan = new();
    private readonly HashSet<InputKey> _keysToRelease = new();

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

    public void OnPieceSpawn(GameState s)
    {
        _plan.Clear();
        if (s.ActivePiece is null) return;
        foreach (var m in FindBestMoves(s.Board, s.ActivePiece))
            _plan.Enqueue(m);
    }

    public void Update()
    {
        // Release keys pressed in the previous frame so that the engine
        // can observe the current tick's key presses.
        foreach (var k in _keysToRelease)
            _input.KeyUp(k);
        _keysToRelease.Clear();

        var piece = _engine.State.ActivePiece;
        if (piece != _lastPiece)
        {
            _lastPiece = piece;
            if (piece != null)
                OnPieceSpawn(_engine.State);
        }

        if (!Enabled || _plan.Count == 0) return;

        int taps = Math.Max(1, SpeedMultiplier);
        while (taps-- > 0 && _plan.Count > 0)
        {
            var key = _plan.Dequeue();
            _input.KeyDown(key);
            _keysToRelease.Add(key);
        }
    }

    public List<InputKey> FindBestMoves(Board board, Tetromino current)
    {
        double bestScore = double.NegativeInfinity;
        int bestX = current.Position.X;
        int bestRot = (int)current.Rotation;

        for (int rot = 0; rot < 4; rot++)
        {
            var piece = current.Clone();
            piece.Rotation = (Rotation)rot;

            for (int x = -2; x < Board.Width + 2; x++)
            {
                var test = piece.Clone();
                test.Position = new PointI(x, test.Position.Y);
                if (board.IsCollision(test))
                    continue;

                while (true)
                {
                    test.Position += new PointI(0, 1);
                    if (board.IsCollision(test))
                    {
                        test.Position -= new PointI(0, 1);
                        break;
                    }
                }

                var temp = CloneBoard(board);
                Place(temp, test);
                int cleared = temp.ClearFullLines();
                ExtractHeights(temp, out int aggHeight, out int holes, out int bumpiness, out int wells);
                double score = 0.76 * cleared - 0.51 * aggHeight - 0.36 * holes - 0.18 * bumpiness - 0.10 * wells;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestX = x;
                    bestRot = rot;
                }
            }
        }

        var actions = new List<InputKey>();

        int dx = bestX - current.Position.X;
        if (dx < 0)
            for (int i = 0; i < -dx; i++) actions.Add(InputKey.Left);
        else if (dx > 0)
            for (int i = 0; i < dx; i++) actions.Add(InputKey.Right);

        int rotDiff = (bestRot - (int)current.Rotation) & 3;
        switch (rotDiff)
        {
            case 1: actions.Add(InputKey.RotateCW); break;
            case 2: actions.Add(InputKey.Rotate180); break;
            case 3: actions.Add(InputKey.RotateCCW); break;
        }

        actions.Add(InputKey.HardDrop);
        return actions;
    }

    private static void Place(Board board, Tetromino piece)
    {
        foreach (var c in piece.Cells)
        {
            var p = c + piece.Position;
            board[p.X, p.Y] = piece.Type;
        }
    }

    private static Board CloneBoard(Board source)
    {
        var b = new Board();
        for (int x = 0; x < Board.Width; x++)
            for (int y = 0; y < Board.TotalHeight; y++)
                b[x, y] = source[x, y];
        return b;
    }

    private static void ExtractHeights(Board board, out int aggHeight, out int holes, out int bumpiness, out int wells)
    {
        int[] heights = new int[Board.Width];
        holes = 0;
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

        aggHeight = heights.Sum();
        bumpiness = 0;
        for (int x = 0; x < Board.Width - 1; x++)
            bumpiness += Math.Abs(heights[x] - heights[x + 1]);

        wells = 0;
        for (int x = 0; x < Board.Width; x++)
        {
            int left = x == 0 ? heights[x + 1] : heights[x - 1];
            int right = x == Board.Width - 1 ? heights[x - 1] : heights[x + 1];
            int min = Math.Min(left, right);
            if (min > heights[x])
                wells += min - heights[x];
        }
    }
}

