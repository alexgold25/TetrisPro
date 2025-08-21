namespace TetrisPro.Core.Models;

public enum PieceType { I, J, L, O, S, T, Z }

public enum Rotation { Spawn, Right, Reverse, Left }

public enum GameStatus { Spawning, Active, LineClear, TopOut, Paused }
