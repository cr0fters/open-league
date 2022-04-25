using Dapper;
using MySqlConnector;

namespace OpenLeague.Repository;

public class LeaguesRepository
{
    private readonly string _connectionString;

    public LeaguesRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<LeagueEntity>> GetLeagues(Guid clubReference)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            return (await connection.QueryAsync<LeagueEntity>(@"
                SELECT
                    l.Reference,
                    l.Name
                FROM
                    League l
                INNER JOIN 
                    Club c on c.ClubId = l.ClubId
                WHERE 
                    c.Reference = @Reference", new
            {
                Reference = clubReference
            })).ToList();
        }
    }

    public async Task<LeagueEntity> GetLeague(Guid leagueReference)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            return (await connection.QuerySingleOrDefaultAsync<LeagueEntity>(@"
                SELECT
                    l.LeagueId,
                    l.Reference,
                    l.Name,
                    l.ClubId,
                    c.Reference as ClubReference,
                    c.Name as ClubName
                FROM
                    League l
                INNER JOIN
                    Club c on c.ClubId = l.ClubId
                WHERE 
                    l.Reference = @Reference", new
            {
                Reference = leagueReference
            }));
        }
    }

    public async Task<LeagueEntity> CreateLeague(int clubId, string name)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            var reference = Guid.NewGuid();
            var leagueId = await connection.QuerySingleAsync<int>(@"
                INSERT INTO
                    League
                    (Reference, 
                    ClubId, 
                    Name)
                VALUES
                    (@Reference, 
                    @ClubId, 
                    @Name);
                SELECT LAST_INSERT_ID();", new
            {
                ClubId = clubId,
                Reference = reference,
                Name = name
            });
            return new LeagueEntity
            {
                LeagueId = leagueId,
                Name = name,
                Reference = reference
            };
        }
    }
}

public class LeagueEntity
{
    public string Name { get; set; }
    public Guid Reference { get; set; }
    public int LeagueId { get; set; }
    public Guid ClubReference { get; set; }
    public string ClubName { get; set; }
}