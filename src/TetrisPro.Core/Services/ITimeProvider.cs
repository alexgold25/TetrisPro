using System;

namespace TetrisPro.Core.Services;

public interface ITimeProvider
{
    DateTimeOffset Now { get; }
}
