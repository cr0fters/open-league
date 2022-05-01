using Dapper;
using MySqlConnector;

namespace OpenLeague.Repository;

public class GameResultsRepository
{
    private const string BaseSql = @"
                SELECT
                    gr.GameId,
                    gr.ClubPlayerId,
                    gr.Position,
                    p.Forename,
                    p.Surname,
                    l.Reference as LeagueReference,
                    p.Reference as PlayerReference,
                    g.Reference as GameReference,
                    g.Season,
                    g.Date as GameDate
                FROM
                    GameResult gr
                INNER JOIN
                    Game g on g.GameId = gr.GameId
                INNER JOIN
                    League l on l.LeagueId = g.LeagueId
                INNER JOIN
                    ClubPlayer cp on cp.ClubPlayerId = gr.ClubPlayerId            
                INNER JOIN
                    Player p on p.PlayerId = cp.PlayerId
                WHERE";

    private readonly string _connectionString;

    public GameResultsRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<GameResultEntity>> ByLeague(Guid leagueReference)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            return (await connection.QueryAsync<GameResultEntity>(
                $@"{BaseSql} l.Reference = @Reference", new
            {
                Reference = leagueReference
            })).ToList();
        }
    }

    public async Task<List<GameResultEntity>> BySeason(Guid leagueReference, int season)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            return (await connection.QueryAsync<GameResultEntity>(
                $@"{BaseSql} g.Season = @Season AND l.Reference = @Reference", new
                {
                    Season = season,
                    Reference = leagueReference
                })).ToList();
        }
    }

    public async Task<List<GameResultEntity>> ByGame(Guid gameReference)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            return (await connection.QueryAsync<GameResultEntity>(
                $@"{BaseSql} g.Reference = @Reference", new
                {
                    Reference = gameReference
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
    public Guid LeagueReference { get; set; }
    public Guid PlayerReference { get; set; }
    public Guid GameReference { get; set; }
    public int Season { get; set; }
    public DateTime GameDate { get; set; }
}