using System;
using System.Collections.Generic;
using System.Linq;
using TetrisPro.Core.Models;

namespace TetrisPro.Core.AI;

/// <summary>Beam search with optional lookahead.</summary>
public class BeamSearch
{
    private readonly int _ply;
    private readonly int _beamWidth;

    public BeamSearch(int ply = 1, int beamWidth = 32)
    {
        _ply = Math.Max(1, ply);
        _beamWidth = Math.Max(1, beamWidth);
    }

    /// <summary>Returns best placement and score for the current piece.</summary>
    public (Placement placement, double score) Search(GameState state, EvalWeights weights)
    {
        if (state.ActivePiece is null)
            return (default, double.NegativeInfinity);

        return SearchRecursive(state.Board, state.ActivePiece, state.NextQueue, weights, _ply, 0);
    }

    private (Placement placement, double score) SearchRecursive(
        Board board,
        Tetromino piece,
        IReadOnlyList<PieceType> queue,
        EvalWeights weights,
        int depth,
        int queueIndex)
    {
        var candidates = new List<(Placement placement, double score, Board board)>();

        for (int rot = 0; rot < 4; rot++)
        {
            var rotated = piece.Clone();
            rotated.Rotation = (Rotation)rot;
            for (int x = -2; x < Board.Width + 2; x++)
            {
                var test = rotated.Clone();
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
                int lines = temp.ClearFullLines();
                int finesse = Finesse.EstimateCost(piece, new Placement(x, rot, true));
                double score = Evaluator.Score(board, new Placement(x, rot, true), temp, weights, lines, finesse);
                candidates.Add((new Placement(x, rot, true), score, temp));
            }
        }

        var ordered = candidates
            .OrderByDescending(c => c.score)
            .Take(_beamWidth)
            .ToList();

        if (depth <= 1 || queueIndex >= queue.Count)
        {
            var best = ordered.FirstOrDefault();
            return (best.placement, best.score);
        }

        double bestScore = double.NegativeInfinity;
        Placement bestPlacement = default;
        foreach (var cand in ordered)
        {
            var nextType = queue[queueIndex];
            var nextPiece = new Tetromino(nextType, "#FFFFFF");
            var (_, childScore) = SearchRecursive(cand.board, nextPiece, queue, weights, depth - 1, queueIndex + 1);
            double total = cand.score + childScore;
            if (total > bestScore)
            {
                bestScore = total;
                bestPlacement = cand.placement;
            }
        }

        return (bestPlacement, bestScore);
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
}
