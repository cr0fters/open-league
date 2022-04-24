namespace OpenLeague.Shared;

public class GetGameResultsRequest
{
    public Guid? GameReference { get; set; }
    public int? Season { get; set; }
}