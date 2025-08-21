namespace TetrisPro.Core.Models;

public readonly record struct PointI(int X, int Y)
{
    public static readonly PointI Zero = new(0,0);
    public static PointI operator +(PointI a, PointI b) => new(a.X + b.X, a.Y + b.Y);
    public static PointI operator -(PointI a, PointI b) => new(a.X - b.X, a.Y - b.Y);
}
