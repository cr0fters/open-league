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

        public LeaguesController(ILogger<LeaguesController> logger)
        {
            _logger = logger;
            _leaguesRepository = new LeaguesRepository("Server=localhost;Database=OpenLeague;Uid=localuser;Pwd=localpass;");
        }

        [HttpGet("clubs/{clubReference}/leagues")]
        public async Task<GetLeaguesResponse> Get(Guid clubReference)
        {
            var leagues = await _leaguesRepository.GetLeagues(clubReference);
            return new GetLeaguesResponse
            {
                Leagues = leagues.Select(leagueEntity=>new League
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
    }
}