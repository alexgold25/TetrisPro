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
            if (piece != null)
            {
                foreach (var act in FindBestMoves(_engine.State.Board, piece.Clone()))
                    _actions.Enqueue(act);
            }
        }

        if (_actions.Count > 0)
        {
            var key = _actions.Dequeue();
            _input.KeyDown(key);
            _release.Add(key);
        }
    }

    private void ReleaseAll()
    {
        foreach (var k in _release.ToList())
        {
            _input.KeyUp(k);
            _release.Remove(k);
        }
        _actions.Clear();
        _lastPiece = null;
    }

    private IEnumerable<InputKey> FindBestMoves(Board board, Tetromino piece)
    {
        var start = new Node(piece.Position, piece.Rotation);
        var queue = new Queue<Node>();
        var parents = new Dictionary<Node, (Node parent, InputKey action)>();
        queue.Enqueue(start);
        parents[start] = (start, default);

        double bestScore = double.NegativeInfinity;
        Node best = start;

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            var p = piece.Clone();
            p.Position = node.Position;
            p.Rotation = node.Rotation;

            var down = p.Clone();
            down.Position += new PointI(0,1);
            bool grounded = board.IsCollision(down);
            if (grounded)
            {
                var b = CloneBoard(board);
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
                    best = node;
                }
            }

            void Try(Node from, InputKey act, Action<Tetromino> move)
            {
                var np = p.Clone();
                move(np);
                if (board.IsCollision(np)) return;
                var n = new Node(np.Position, np.Rotation);
                if (parents.ContainsKey(n)) return;
                queue.Enqueue(n);
                parents[n] = (from, act);
            }

            Try(node, InputKey.Left, t => t.Position += new PointI(-1,0));
            Try(node, InputKey.Right, t => t.Position += new PointI(1,0));
            Try(node, InputKey.SoftDrop, t => t.Position += new PointI(0,1));
            Try(node, InputKey.RotateCW, t => t.RotateCW());
            Try(node, InputKey.RotateCCW, t => t.RotateCCW());
            Try(node, InputKey.Rotate180, t => t.Rotate180());
        }

        var actions = new List<InputKey>();
        var cur = best;
        while (cur != start)
        {
            var info = parents[cur];
            actions.Add(info.action);
            cur = info.parent;
        }
        actions.Reverse();
        actions.Add(InputKey.HardDrop);
        return actions;
    }

    private readonly record struct Node(PointI Position, Rotation Rotation);

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

