using System;
using System.Collections.Generic;
using TetrisPro.Core.Models;

namespace TetrisPro.Core.Services;

/// <summary>7-bag randomizer using Fisher-Yates shuffle.</summary>
public class SevenBagRandomizer : IRandomizer
{
    private readonly Queue<PieceType> _queue = new();
    private readonly Random _random;

    public SevenBagRandomizer(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
        Refill();
    }

    private void Refill()
    {
        var pieces = new List<PieceType>
        {
            PieceType.I, PieceType.J, PieceType.L, PieceType.O, PieceType.S, PieceType.T, PieceType.Z
        };
        for (int i = pieces.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (pieces[i], pieces[j]) = (pieces[j], pieces[i]);
        }
        foreach (var p in pieces)
            _queue.Enqueue(p);
    }

    public PieceType Next()
    {
        if (_queue.Count == 0) Refill();
        return _queue.Dequeue();
    }
}
