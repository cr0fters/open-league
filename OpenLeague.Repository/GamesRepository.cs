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
}