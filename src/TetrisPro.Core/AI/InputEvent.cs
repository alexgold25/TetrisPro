using TetrisPro.Core.Services;

namespace TetrisPro.Core.AI;

/// <summary>Represents a single key press at a given timestamp.</summary>
public readonly record struct InputEvent(InputKey Key, int TimeFromStartMs);
