namespace TetrisPro.Core.Engine;

public static class Scoring
{
    public static int Lines(int count) => count switch
    {
        1 => 100,
        2 => 300,
        3 => 500,
        4 => 800,
        _ => 0
    };

    public static int SoftDrop(int cells) => cells;
    public static int HardDrop(int cells) => cells * 2;
}
