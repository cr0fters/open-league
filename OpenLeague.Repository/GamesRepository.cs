using Dapper;
using MySqlConnector;

namespace OpenLeague.Repository;

public class GamesRepository
{
    private readonly string _connectionString;

    public GamesRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<GameEntity>> RetrieveGames(Guid leagueReference)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            return (await connection.QueryAsync<GameEntity>(@"
                SELECT
                    g.GameId,
                    g.Reference,
                    g.LeagueId,
                    g.Date,
                    g.Season
                FROM
                    Game g
                INNER JOIN 
                    League l on l.LeagueId = g.LeagueId
                WHERE
                    l.Reference = @Reference", new
            {
                Reference = leagueReference
            })).ToList();
        }
    }

    public async Task<GameEntity> RetrieveGame(Guid gameReference)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            return await connection.QuerySingleAsync<GameEntity>(@"
                SELECT
                    g.GameId,
                    g.Reference,
                    g.LeagueId,
                    g.Date,
                    g.Season,
                    c.Reference as ClubReference
                FROM
                    Game g
                INNER JOIN 
                    League l on l.LeagueId = g.LeagueId
                INNER JOIN
                    Club c on c.ClubId = l.ClubId
                WHERE
                    g.Reference = @Reference", new
            {
                Reference = gameReference
            });
        }
    }

    public async Task<GameEntity> CreateGame(int leagueId, DateTime date, int season)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            var reference = Guid.NewGuid();
            var gameId = await connection.QuerySingleAsync<int>(@"
                INSERT INTO
                    Game
                    (Reference, 
                    LeagueId, 
                    Date,
                    Season)
                VALUES
                    (@Reference, 
                    @LeagueId, 
                    @Date,
                    @Season);
                SELECT LAST_INSERT_ID();", new
            {
                Reference = reference,
                LeagueId = leagueId,
                Date = date,
                Season = season
            });
            return new GameEntity
            {
                Reference = reference,
                GameId = gameId,
                LeagueId = leagueId,
                Date = date,
                Season = season
            };
        }
    }
}

public class GameEntity
{
    public int GameId { get; set; }
    public Guid Reference { get; set; }
    public int LeagueId { get; set; }
    public DateTime Date { get; set; }
    public int Season { get; set; }
    public Guid ClubReference { get; set; }
}