using System.Collections.Generic;
using TetrisPro.Core.Models;

namespace TetrisPro.Core.Engine;

/// <summary>Static SRS shapes and wall kick data.</summary>
public static class SrsData
{
    public static readonly Dictionary<PieceType, PointI[][]> Shapes = new()
    {
        [PieceType.I] = new[]
        {
            new [] { new PointI(-1,0), new PointI(0,0), new PointI(1,0), new PointI(2,0) },
            new [] { new PointI(1,1), new PointI(1,0), new PointI(1,-1), new PointI(1,-2) },
            new [] { new PointI(-1,-1), new PointI(0,-1), new PointI(1,-1), new PointI(2,-1) },
            new [] { new PointI(0,1), new PointI(0,0), new PointI(0,-1), new PointI(0,-2) }
        },
        [PieceType.J] = new[]
        {
            new [] { new PointI(-1,0), new PointI(0,0), new PointI(1,0), new PointI(-1,1) },
            new [] { new PointI(0,-1), new PointI(0,0), new PointI(0,1), new PointI(1,-1) },
            new [] { new PointI(-1,0), new PointI(0,0), new PointI(1,0), new PointI(1,-1) },
            new [] { new PointI(0,-1), new PointI(0,0), new PointI(0,1), new PointI(-1,1) }
        },
        [PieceType.L] = new[]
        {
            new [] { new PointI(-1,0), new PointI(0,0), new PointI(1,0), new PointI(1,1) },
            new [] { new PointI(0,-1), new PointI(0,0), new PointI(0,1), new PointI(1,1) },
            new [] { new PointI(-1,-1), new PointI(-1,0), new PointI(0,0), new PointI(1,0) },
            new [] { new PointI(-1,-1), new PointI(0,-1), new PointI(0,0), new PointI(0,1) }
        },
        [PieceType.O] = new[]
        {
            new [] { new PointI(0,0), new PointI(1,0), new PointI(0,1), new PointI(1,1) },
            new [] { new PointI(0,0), new PointI(1,0), new PointI(0,1), new PointI(1,1) },
            new [] { new PointI(0,0), new PointI(1,0), new PointI(0,1), new PointI(1,1) },
            new [] { new PointI(0,0), new PointI(1,0), new PointI(0,1), new PointI(1,1) }
        },
        [PieceType.S] = new[]
        {
            new [] { new PointI(0,0), new PointI(1,0), new PointI(-1,1), new PointI(0,1) },
            new [] { new PointI(0,-1), new PointI(0,0), new PointI(1,0), new PointI(1,1) },
            new [] { new PointI(0,0), new PointI(1,0), new PointI(-1,1), new PointI(0,1) },
            new [] { new PointI(0,-1), new PointI(0,0), new PointI(1,0), new PointI(1,1) }
        },
        [PieceType.T] = new[]
        {
            new [] { new PointI(-1,0), new PointI(0,0), new PointI(1,0), new PointI(0,1) },
            new [] { new PointI(0,-1), new PointI(0,0), new PointI(1,0), new PointI(0,1) },
            new [] { new PointI(0,-1), new PointI(-1,0), new PointI(0,0), new PointI(1,0) },
            new [] { new PointI(0,-1), new PointI(-1,0), new PointI(0,0), new PointI(0,1) }
        },
        [PieceType.Z] = new[]
        {
            new [] { new PointI(-1,0), new PointI(0,0), new PointI(0,1), new PointI(1,1) },
            new [] { new PointI(1,-1), new PointI(0,0), new PointI(1,0), new PointI(0,1) },
            new [] { new PointI(-1,0), new PointI(0,0), new PointI(0,1), new PointI(1,1) },
            new [] { new PointI(1,-1), new PointI(0,0), new PointI(1,0), new PointI(0,1) }
        }
    };

    public static readonly Dictionary<(Rotation from, Rotation to), PointI[]> JLSTZKicks = new()
    {
        [(Rotation.Spawn, Rotation.Right)] = new[]
        {
            new PointI(0,0), new PointI(-1,0), new PointI(-1,1), new PointI(0,-2), new PointI(-1,-2)
        },
        [(Rotation.Right, Rotation.Spawn)] = new[]
        {
            new PointI(0,0), new PointI(1,0), new PointI(1,-1), new PointI(0,2), new PointI(1,2)
        },
        [(Rotation.Right, Rotation.Reverse)] = new[]
        {
            new PointI(0,0), new PointI(1,0), new PointI(1,-1), new PointI(0,2), new PointI(1,2)
        },
        [(Rotation.Reverse, Rotation.Right)] = new[]
        {
            new PointI(0,0), new PointI(-1,0), new PointI(-1,1), new PointI(0,-2), new PointI(-1,-2)
        },
        [(Rotation.Reverse, Rotation.Left)] = new[]
        {
            new PointI(0,0), new PointI(1,0), new PointI(1,1), new PointI(0,-2), new PointI(1,-2)
        },
        [(Rotation.Left, Rotation.Reverse)] = new[]
        {
            new PointI(0,0), new PointI(-1,0), new PointI(-1,-1), new PointI(0,2), new PointI(-1,2)
        },
        [(Rotation.Left, Rotation.Spawn)] = new[]
        {
            new PointI(0,0), new PointI(-1,0), new PointI(-1,-1), new PointI(0,2), new PointI(-1,2)
        },
        [(Rotation.Spawn, Rotation.Left)] = new[]
        {
            new PointI(0,0), new PointI(1,0), new PointI(1,1), new PointI(0,-2), new PointI(1,-2)
        }
    };

    public static readonly Dictionary<(Rotation from, Rotation to), PointI[]> IKicks = new()
    {
        [(Rotation.Spawn, Rotation.Right)] = new[]
        {
            new PointI(0,0), new PointI(-2,0), new PointI(1,0), new PointI(-2,-1), new PointI(1,2)
        },
        [(Rotation.Right, Rotation.Spawn)] = new[]
        {
            new PointI(0,0), new PointI(2,0), new PointI(-1,0), new PointI(2,1), new PointI(-1,-2)
        },
        [(Rotation.Right, Rotation.Reverse)] = new[]
        {
            new PointI(0,0), new PointI(-1,0), new PointI(2,0), new PointI(-1,2), new PointI(2,-1)
        },
        [(Rotation.Reverse, Rotation.Right)] = new[]
        {
            new PointI(0,0), new PointI(1,0), new PointI(-2,0), new PointI(1,-2), new PointI(-2,1)
        },
        [(Rotation.Reverse, Rotation.Left)] = new[]
        {
            new PointI(0,0), new PointI(2,0), new PointI(-1,0), new PointI(2,1), new PointI(-1,-2)
        },
        [(Rotation.Left, Rotation.Reverse)] = new[]
        {
            new PointI(0,0), new PointI(-2,0), new PointI(1,0), new PointI(-2,-1), new PointI(1,2)
        },
        [(Rotation.Left, Rotation.Spawn)] = new[]
        {
            new PointI(0,0), new PointI(1,0), new PointI(-2,0), new PointI(1,-2), new PointI(-2,1)
        },
        [(Rotation.Spawn, Rotation.Left)] = new[]
        {
            new PointI(0,0), new PointI(-1,0), new PointI(2,0), new PointI(-1,2), new PointI(2,-1)
        }
    };
}
