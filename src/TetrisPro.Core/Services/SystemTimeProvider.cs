using System;

namespace TetrisPro.Core.Services;

public class SystemTimeProvider : ITimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}
