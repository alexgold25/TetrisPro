using System;
using TetrisPro.Core.Models;

namespace TetrisPro.Core.AI;

/// <summary>Very simple beam search over placements.</summary>
public class BeamSearch
{
    private readonly int _ply;
    private readonly int _beamWidth;

    public BeamSearch(int ply = 1, int beamWidth = 32)
    {
        _ply = ply;
        _beamWidth = beamWidth;
    }

    /// <summary>Returns best placement and score for the current piece.</summary>
    public (Placement placement, double score) Search(GameState state, EvalWeights weights)
    {
        if (state.ActivePiece == null)
            return (default, double.NegativeInfinity);

        var piece = state.ActivePiece;
        double best = double.NegativeInfinity;
        Placement bestPlacement = new();

        for (int rot = 0; rot < 4; rot++)
        {
            var rotated = piece.Clone();
            rotated.Rotation = (Rotation)rot;
            for (int x = -2; x < Board.Width + 2; x++)
            {
                var test = rotated.Clone();
                test.Position = new PointI(x, test.Position.Y);
                if (state.Board.IsCollision(test))
                    continue;
                while (true)
                {
                    test.Position += new PointI(0, 1);
                    if (state.Board.IsCollision(test))
                    {
                        test.Position -= new PointI(0, 1);
                        break;
                    }
                }
                var temp = CloneBoard(state.Board);
                Place(temp, test);
                int lines = temp.ClearFullLines();
                int finesse = Finesse.EstimateCost(piece, new Placement(x, rot, true));
                double score = Evaluator.Score(state.Board, new Placement(x, rot, true), temp, weights, lines, finesse);
                if (score > best)
                {
                    best = score;
                    bestPlacement = new Placement(x, rot, true);
                }
            }
        }
        return (bestPlacement, best);
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
