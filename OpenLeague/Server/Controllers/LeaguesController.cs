using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using OpenLeague.Repository;
using OpenLeague.Shared;

namespace OpenLeague.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class LeaguesController : ControllerBase
    {
        private readonly ILogger<LeaguesController> _logger;
        private readonly LeaguesRepository _leaguesRepository;
        private readonly ClubsRepository _clubsRepository;
        private readonly GamesRepository _gamesRepository;

        public LeaguesController(ILogger<LeaguesController> logger)
        {
            _logger = logger;
            _leaguesRepository = new LeaguesRepository("Server=localhost;Database=OpenLeague;Uid=localuser;Pwd=localpass;");
            _clubsRepository = new ClubsRepository("Server=localhost;Database=OpenLeague;Uid=localuser;Pwd=localpass;");
            _gamesRepository = new GamesRepository("Server=localhost;Database=OpenLeague;Uid=localuser;Pwd=localpass;");

        }

        [HttpGet("clubs/{clubReference}/leagues")]
        public async Task<GetLeaguesResponse> Get(Guid clubReference)
        {
            var leagues = await _leaguesRepository.GetLeagues(clubReference);
            return new GetLeaguesResponse
            {
                Leagues = leagues.Select(leagueEntity => new League
                {
                    Name = leagueEntity.Name,
                    Reference = leagueEntity.Reference
                }).ToList()
            };
        }

        [HttpGet("leagues/{leagueReference}")]
        public async Task<ActionResult<GetLeagueResponse>> GetLeague(Guid leagueReference)
        {
            var league = await _leaguesRepository.GetLeague(leagueReference);
            if (league == null)
            {
                return NotFound();
            }

            return new GetLeagueResponse
            {
                League = new League
                {
                    Name = league.Name,
                    Reference = league.Reference
                }
            };
        }

        [HttpPost("clubs/{clubReference}/leagues")]
        public async Task<ActionResult<CreateLeagueResponse>> Post([FromRoute] Guid clubReference, CreateLeagueRequest createLeagueRequest)
        {
            const int seasons = 4;
            const int gamesPerSeason = 12;
            var nextGameDate = createLeagueRequest.StartDate;
            var club = await _clubsRepository.GetClub(clubReference);
            if (club == null)
            {
                return NotFound();
            }
            var leagueEntity = await _leaguesRepository.CreateLeague(club.ClubId, createLeagueRequest.Name);
            for (var season = 1; season <= seasons; season++)
            {
                for (var game = 1; game <= gamesPerSeason; game++)
                {
                    await _gamesRepository.CreateGame(leagueEntity.LeagueId, nextGameDate, season);
                    nextGameDate = nextGameDate.AddDays(7);
                }
            }
            return new CreateLeagueResponse
            {
                Reference = leagueEntity.Reference
            };
        }
    }
}