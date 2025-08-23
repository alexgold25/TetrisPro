using System;
using System.Linq;
using TetrisPro.Core.Models;

namespace TetrisPro.Core.AI;

/// <summary>Extracts board features used by the evaluator.</summary>
public static class Features
{
    public readonly struct Result
    {
        public readonly int AggregateHeight;
        public readonly int Holes;
        public readonly int Bumpiness;
        public readonly int WellSum;
        public Result(int agg, int holes, int bump, int wells)
        {
            AggregateHeight = agg;
            Holes = holes;
            Bumpiness = bump;
            WellSum = wells;
        }
    }

    /// <summary>Calculates aggregate height, holes, bumpiness and wells.</summary>
    public static Result Extract(Board board)
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

        int aggHeight = heights.Sum();
        int bumpiness = 0;
        for (int x = 0; x < Board.Width - 1; x++)
            bumpiness += Math.Abs(heights[x] - heights[x + 1]);

        int wells = 0;
        for (int x = 0; x < Board.Width; x++)
        {
            int left = x == 0 ? heights[x + 1] : heights[x - 1];
            int right = x == Board.Width - 1 ? heights[x - 1] : heights[x + 1];
            int min = Math.Min(left, right);
            if (min > heights[x])
                wells += min - heights[x];
        }

        return new Result(aggHeight, holes, bumpiness, wells);
    }
}
