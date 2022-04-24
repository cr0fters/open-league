using Dapper;
using MySqlConnector;

namespace OpenLeague.Repository;

public class ClubsRepository
{
    private readonly string _connectionString;

    public ClubsRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<ClubEntity>> GetClubs()
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            return (await connection.QueryAsync<ClubEntity>(@"
                SELECT
                    Reference,
                    Name
                FROM
                    Club")).ToList();
        }
    }

    public async Task<ClubEntity> GetClub(Guid reference)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            return await connection.QuerySingleOrDefaultAsync<ClubEntity>(@"
                SELECT
                    ClubId,
                    Reference,
                    Name
                FROM
                    Club
                WHERE
                    Reference = @Reference", new
            {
                Reference = reference
            });
        }
    }
}

public class ClubEntity
{
    public int ClubId { get; set; }
    public string Name { get; set; }
    public Guid Reference { get; set; }
}