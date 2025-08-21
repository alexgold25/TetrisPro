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
        60.0/48, 60.0/43, 60.0/38, 60.0/33, 60.0/28,
        60.0/23, 60.0/18, 60.0/13, 60.0/8, 60.0/6,
        60.0/5, 60.0/4, 60.0/3, 60.0/2, 60.0/1,
        60.0*2, 60.0*3, 60.0*4, 60.0*5, 60.0*6,
        60.0*7, 60.0*8, 60.0*9, 60.0*10, 60.0*11,
        60.0*12, 60.0*13, 60.0*14, 60.0*15, 60.0*20
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
