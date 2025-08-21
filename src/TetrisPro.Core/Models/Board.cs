using System;
using System.Collections.Generic;
using System.Linq;

namespace TetrisPro.Core.Models;

public class Board
{
    public const int Width = 10;
    public const int VisibleHeight = 20;
    public const int HiddenRows = 2;
    public const int TotalHeight = VisibleHeight + HiddenRows;

    private readonly PieceType?[,] _cells = new PieceType?[Width, TotalHeight];

    public PieceType? this[int x, int y]
    {
        get => _cells[x, y];
        set => _cells[x, y] = value;
    }

    public bool IsInside(int x, int y) => x >= 0 && x < Width && y >= 0 && y < TotalHeight;

    public bool IsCollision(Tetromino piece)
    {
        foreach (var cell in piece.Cells)
        {
            var target = cell + piece.Position;
            if (!IsInside(target.X, target.Y) || _cells[target.X, target.Y].HasValue)
                return true;
        }
        return false;
    }

    public int ClearFullLines()
    {
        var cleared = new List<int>();
        for (int y = 0; y < TotalHeight; y++)
        {
            if (Enumerable.Range(0, Width).All(x => _cells[x, y].HasValue))
            {
                cleared.Add(y);
            }
        }
        foreach (var y in cleared)
        {
            for (int row = y; row > 0; row--)
                for (int x = 0; x < Width; x++)
                    _cells[x, row] = _cells[x, row - 1];
            for (int x = 0; x < Width; x++) _cells[x, 0] = null;
        }
        return cleared.Count;
    }

    public IEnumerable<(int x, int y, PieceType? type)> GetCells()
    {
        for (int y = HiddenRows; y < TotalHeight; y++)
            for (int x = 0; x < Width; x++)
                yield return (x, y - HiddenRows, _cells[x, y]);
    }
}
