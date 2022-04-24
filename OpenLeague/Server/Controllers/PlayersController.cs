using Microsoft.AspNetCore.Mvc;
using OpenLeague.Repository;
using OpenLeague.Shared;

namespace OpenLeague.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class PlayersController : ControllerBase
    {
        private readonly ILogger<PlayersController> _logger;
        private readonly PlayersRepository _playersRepository;
        private readonly ClubsRepository _clubsRepository;

        public PlayersController(ILogger<PlayersController> logger)
        {
            _logger = logger;
            _playersRepository = new PlayersRepository("Server=localhost;Database=OpenLeague;Uid=localuser;Pwd=localpass;");
            _clubsRepository = new ClubsRepository("Server=localhost;Database=OpenLeague;Uid=localuser;Pwd=localpass;");
        }

        [HttpPost("clubs/{clubReference}/players")]
        public async Task<ActionResult<CreatePlayerResponse>> Post([FromRoute]Guid clubReference, [FromBody]CreatePlayerRequest createPlayerRequest)
        {
            var club = await _clubsRepository.GetClub(clubReference);
            if (club == null)
            {
                return NotFound();
            }
            var playerEntity = await _playersRepository.CreatePlayer(createPlayerRequest.Forename, createPlayerRequest.Surname, club.ClubId);
            var response = new CreatePlayerResponse
            {
                Reference = playerEntity.Reference,
                Forename = createPlayerRequest.Forename,
                Surname = createPlayerRequest.Surname
            };
            return response;
        }

        [HttpGet("clubs/{clubReference}/players")]
        public async Task<GetPlayersResponse> Get(Guid clubReference)
        {
            var players = await _playersRepository.GetPlayers(clubReference);
            var response = new GetPlayersResponse
            {
                Players = players.Select(playerEntity=>new Player
                {
                    Name = playerEntity.Forename + " " + playerEntity.Surname,
                    Reference = playerEntity.Reference
                }).ToList()
            };
            return response;
        }
    }

    
}