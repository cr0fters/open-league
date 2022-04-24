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
                    Reference,
                    Name
                FROM
                    League
                WHERE 
                    Reference = @Reference", new
            {
                Reference = leagueReference
            }));
        }
    }
}

public class LeagueEntity
{
    public string Name { get; set; }
    public Guid Reference { get; set; }
}