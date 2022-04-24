namespace OpenLeague.Shared;

public class GameResult
{
    public int Points { get; set; }
    public Player Player { get; set; }
    public Guid GameReference { get; set; }
    public int Season { get; set; }
}