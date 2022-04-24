using Microsoft.AspNetCore.Mvc;
using OpenLeague.Repository;
using OpenLeague.Shared;

namespace OpenLeague.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class ResultsController : ControllerBase
    {
        private readonly ILogger<ResultsController> _logger;
        private readonly GameResultsRepository _gameResultsRepository;

        public ResultsController(ILogger<ResultsController> logger)
        {
            _logger = logger;
            _gameResultsRepository = new GameResultsRepository("Server=localhost;Database=OpenLeague;Uid=localuser;Pwd=localpass;");
        }

        [HttpGet("leagues/{leagueReference}/results")]
        public async Task<GetGameResultsResponse> Get(Guid leagueReference, [FromQuery]GetGameResultsRequest getGameResultsRequest)
        {

            var gameResultEntities = (await _gameResultsRepository.RetrieveGameResults(leagueReference, getGameResultsRequest.GameReference, getGameResultsRequest.Season))
                .OrderBy(gameResultEntity=>gameResultEntity.Position).ToList();
            var results = gameResultEntities.Select((gameResultEntity, count) =>
            {
                var position = count + 1;
                return new GameResult
                {
                    GameReference = gameResultEntity.GameReference,
                    Season = gameResultEntity.Season,
                    Player = new Player
                    {
                        Name = gameResultEntity.Forename + " " + gameResultEntity.Surname,
                        Reference = gameResultEntity.PlayerReference,
                    },
                    Points = CalculatePoints(position)
                };
            }).ToList();
            var response = new GetGameResultsResponse
            {
                Results = results
            };
            return response;
        }

        private static int CalculatePoints(int position)
        {
            return position switch
            {
                1 => 100,
                2 => 80,
                3 => 70,
                4 => 60,
                5 => 50,
                6 => 40,
                7 => 30,
                8 => 20,
                _ => 10
            };
        }
    }
}