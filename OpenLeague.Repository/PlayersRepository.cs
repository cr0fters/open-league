using Dapper;
using MySqlConnector;

namespace OpenLeague.Repository;

public class PlayersRepository
{
    private readonly string _connectionString;

    public PlayersRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PlayerEntity> CreatePlayer(string forename, string surname, int clubId)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            var reference = Guid.NewGuid();
            var playerId = await connection.QuerySingleAsync<int>(@"
                INSERT INTO
                    Player
                    (Reference,
                    Forename,
                    Surname)
                VALUES
                    (@Reference,
                    @Forename,
                    @Surname);
                SELECT LAST_INSERT_ID();", new
            {
                Reference = reference,
                Forename = forename,
                Surname = surname
            });
            await connection.ExecuteAsync(@"
                INSERT INTO
                    ClubPlayer
                    (ClubId,
                    PlayerId)
                VALUES
                    (@ClubId,
                    @PlayerId)", new
            {
                ClubId = clubId,
                PlayerId = playerId
            });
            return new PlayerEntity
            {
                Surname = surname,
                Forename = forename,
                Reference = reference,
                PlayerId = playerId
            };
        }
    }

    public async Task<List<PlayerEntity>> GetPlayers(Guid clubReference)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            return (await connection.QueryAsync<PlayerEntity>(@"
                SELECT
                    p.PlayerId,
                    p.Reference,
                    p.Forename,
                    p.Surname
                FROM
                    Player p
                INNER JOIN
                    ClubPlayer cp on cp.PlayerId = p.PlayerId
                INNER JOIN
                    Club c on c.ClubId = cp.ClubId
                WHERE 
                    c.Reference = @Reference", new
            {
                Reference = clubReference
            })).ToList();
        }
    }
}

public class PlayerEntity
{
    public string Forename { get; set; }
    public string Surname { get; set; }
    public Guid Reference { get; set; }
    public int PlayerId { get; set; }
}