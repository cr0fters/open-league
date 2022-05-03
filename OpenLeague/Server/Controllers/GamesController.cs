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
        private readonly GameResultsRepository _gameResultsRepository;
        private readonly PlayersRepository _playersRepository;

        public GamesController(ILogger<GamesController> logger)
        {
            _logger = logger;
            _gamesRepository = new GamesRepository("Server=localhost;Database=OpenLeague;Uid=localuser;Pwd=localpass;");
            _gameResultsRepository = new GameResultsRepository("Server=localhost;Database=OpenLeague;Uid=localuser;Pwd=localpass;");
            _playersRepository = new PlayersRepository("Server=localhost;Database=OpenLeague;Uid=localuser;Pwd=localpass;");
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

        [HttpPost("games/{gameReference}/results")]
        public async Task<ActionResult<GetGamesResponse>> AddResult(Guid gameReference, AddGameResultRequest addGameResultRequest)
        {
            var game = await _gamesRepository.RetrieveGame(gameReference);
            var player = await _playersRepository.GetPlayer(addGameResultRequest.PlayerReference, game.ClubReference);
            await _gameResultsRepository.AddResult(game.GameId, player.ClubPlayerId, addGameResultRequest.Position);
            return Ok();
        }
    }
}