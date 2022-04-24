using Dapper;
using MySqlConnector;

namespace OpenLeague.Repository;

public class GameResultsRepository
{
    private readonly string _connectionString;

    public GameResultsRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<GameResultEntity>> RetrieveGameResults(Guid leagueReference, Guid? gameReference = null, int? season = null)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            return (await connection.QueryAsync<GameResultEntity>(@"
                SELECT
                    gr.GameId,
                    gr.ClubPlayerId,
                    gr.Position,
                    p.Forename,
                    p.Surname,
                    p.Reference as PlayerReference,
                    g.Reference as GameReference,
                    g.Season
                FROM
                    GameResult gr
                INNER JOIN
                    Game g on gr.GameId = gr.GameId
                INNER JOIN
                    League l on l.LeagueId = g.LeagueId
                INNER JOIN
                    ClubPlayer cp on cp.ClubPlayerId = gr.ClubPlayerId            
                INNER JOIN
                    Player p on p.PlayerId = cp.PlayerId
                WHERE
                    l.Reference = @LeagueReference
                AND
                    (@GameReference IS NULL OR g.Reference = @GameReference)
                AND
                    (@Season IS NULL OR g.Season = @Season)", new
            {
                LeagueReference = leagueReference,
                GameReference = gameReference,
                Season = season
            })).ToList();
        }
    }
}

public class GameResultEntity
{
    public int GameId { get; set; }
    public int ClubPlayerId { get; set; }
    public int Position { get; set; }
    public string Forename { get; set; }
    public string Surname { get; set; }
    public Guid PlayerReference { get; set; }
    public Guid GameReference { get; set; }
    public int Season { get; set; }
}