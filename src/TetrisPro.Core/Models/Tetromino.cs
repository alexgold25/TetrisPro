using System.Collections.Generic;
using TetrisPro.Core.Engine;

namespace TetrisPro.Core.Models;

/// <summary>Represents a tetromino piece.</summary>
public class Tetromino
{
    public PieceType Type { get; }
    public Rotation Rotation { get; set; }
    public PointI Position { get; set; }
    public string Color { get; }

    public Tetromino(PieceType type, string color)
    {
        Type = type;
        Color = color;
        Rotation = Models.Rotation.Spawn;
        Position = new PointI(4, 0); // spawn near center
    }

    public Tetromino Clone() => new(Type, Color) { Rotation = this.Rotation, Position = this.Position };

    public IReadOnlyList<PointI> Cells => SrsData.Shapes[Type][(int)Rotation];

    public void RotateCW() => Rotation = (Rotation)(((int)Rotation + 1) & 3);
    public void RotateCCW() => Rotation = (Rotation)(((int)Rotation + 3) & 3);
    public void Rotate180() => Rotation = (Rotation)(((int)Rotation + 2) & 3);
}
