using System;
using TetrisPro.Core.Config;
using TetrisPro.Core.Models;

namespace TetrisPro.Core.Engine;

public interface IGameEngine
{
    GameState State { get; }
    void StartNewGame(GameConfig config);
    void Pause();
    void Resume();
    void Update(TimeSpan delta);
}
