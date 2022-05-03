using Microsoft.AspNetCore.Mvc;
using OpenLeague.Repository;
using OpenLeague.Shared;

namespace OpenLeague.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class StandingsController : ControllerBase
    {
        private readonly ILogger<StandingsController> _logger;
        private readonly GameResultsRepository _gameResultsRepository;

        public StandingsController(ILogger<StandingsController> logger)
        {
            _logger = logger;
            _gameResultsRepository = new GameResultsRepository("Server=localhost;Database=OpenLeague;Uid=localuser;Pwd=localpass;");
        }

        [HttpGet("games/{gameReference}/standings")]
        public async Task<GetStandingsResponse> GetGameStandings(Guid gameReference)
        {
            var gameResultEntities = (await _gameResultsRepository.ByGame(gameReference));
            return new GetStandingsResponse
            {
                Standings = BuildStandings(gameResultEntities)
            };
        }

        [HttpGet("leagues/{leagueReference}/standings")]
        public async Task<GetStandingsResponse> GetLeagueStandings(Guid leagueReference)
        {
            var gameResultEntities = await _gameResultsRepository.ByLeague(leagueReference);
            var standings = new List<Standing>();
            foreach (var gameResultGroup in gameResultEntities.GroupBy(gameResultEntity => gameResultEntity.GameReference).Select(BuildStandings))
            {
                foreach (var groupedStanding in gameResultGroup.ToList())
                {
                    if (standings.Any(standing => standing.Player.Reference == groupedStanding.Player.Reference))
                    {
                        standings.Single(standing => standing.Player.Reference == groupedStanding.Player.Reference).Points += groupedStanding.Points;
                    }
                    else
                    {
                        standings.Add(groupedStanding);
                    }
                }
            }
            return new GetStandingsResponse
            {
                Standings = standings
            };
        }

        [HttpGet("leagues/{leagueReference}/seasons/{season}/standings")]
        public async Task<GetStandingsResponse> GetSeasonStandings(Guid leagueReference, int season)
        {
            var gameResultEntities = await _gameResultsRepository.BySeason(leagueReference, season);
            var standings = new List<Standing>();
            foreach (var gameResultGroup in gameResultEntities.GroupBy(gameResultEntity => gameResultEntity.GameReference).Select(BuildStandings))
            {
                foreach (var groupedStanding in gameResultGroup.ToList())
                {
                    if (standings.Any(standing => standing.Player.Reference == groupedStanding.Player.Reference))
                    {
                        standings.Single(standing => standing.Player.Reference == groupedStanding.Player.Reference).Points += groupedStanding.Points;
                    }
                    else
                    {
                        standings.Add(groupedStanding);
                    }
                }
            }
            return new GetStandingsResponse
            {
                Standings = standings
            };
        }

        private static List<Standing> BuildStandings(IEnumerable<GameResultEntity> gameResultEntities)
        {
            return gameResultEntities.OrderBy(gameResultEntity=>gameResultEntity.Position).Select((gameResultEntity, count) =>
            {
                var position = count + 1;
                return new Standing
                {
                    Player = new Player
                    {
                        Name = gameResultEntity.Forename + " " + gameResultEntity.Surname,
                        Reference = gameResultEntity.PlayerReference,
                    },
                    Points = CalculatePoints(position)
                };
            }).ToList();
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