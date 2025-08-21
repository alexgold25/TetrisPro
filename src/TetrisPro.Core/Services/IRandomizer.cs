using TetrisPro.Core.Models;

namespace TetrisPro.Core.Services;

public interface IRandomizer
{
    PieceType Next();
}
