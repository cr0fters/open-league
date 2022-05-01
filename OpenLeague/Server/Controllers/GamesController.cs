using Microsoft.AspNetCore.Mvc;
using OpenLeague.Repository;
using OpenLeague.Shared;

namespace OpenLeague.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class GamesController : ControllerBase
    {
        private readonly ILogger<GamesController> _logger;
        private readonly GamesRepository _gamesRepository;

        public GamesController(ILogger<GamesController> logger)
        {
            _logger = logger;
            _gamesRepository = new GamesRepository("Server=localhost;Database=OpenLeague;Uid=localuser;Pwd=localpass;");
        }

        [HttpGet("leagues/{leagueReference}/games")]
        public async Task<GetGamesResponse> Get(Guid leagueReference)
        {
            var games = await _gamesRepository.RetrieveGames(leagueReference);
            return new GetGamesResponse
            {
                Games = games.Select(gameEntity=>new Game
                {
                    Date = gameEntity.Date,
                    Reference = gameEntity.Reference,
                    Season = gameEntity.Season
                }).ToList()
            };
        }
    }
}