using System;
using System.Collections.Generic;

namespace TetrisPro.Core.Config;

/// <summary>
/// Game wide configuration values. These can later be loaded from json or user settings.
/// </summary>
public class GameConfig
{
    /// <summary>
    /// Gravity speed in cells per second for levels 0-29. Values are roughly based on classic guideline tables.
    /// </summary>
    public IReadOnlyList<double> GravityByLevel { get; init; } = new double[]
    {
        1.0/48, 1.0/43, 1.0/38, 1.0/33, 1.0/28,
        1.0/23, 1.0/18, 1.0/13, 1.0/8, 1.0/6,
        1.0/5, 1.0/4, 1.0/3, 1.0/2, 1.0,
        2.0, 3.0, 4.0, 5.0, 6.0,
        7.0, 8.0, 9.0, 10.0, 11.0,
        12.0, 13.0, 14.0, 15.0, 20.0
    };

    /// <summary>Delay before piece locks after touching ground.</summary>
    public TimeSpan LockDelay { get; init; } = TimeSpan.FromMilliseconds(500);

    /// <summary>Delay before DAS (auto shift) repeats.</summary>
    public TimeSpan Das { get; init; } = TimeSpan.FromMilliseconds(170);

    /// <summary>Auto repeat rate for horizontal movement.</summary>
    public TimeSpan Arr { get; init; } = TimeSpan.FromMilliseconds(30);

    /// <summary>Number of preview pieces in next queue.</summary>
    public int PreviewCount { get; init; } = 5;
}
