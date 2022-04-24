using Microsoft.AspNetCore.Mvc;
using OpenLeague.Repository;
using OpenLeague.Shared;

namespace OpenLeague.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class ClubsController : ControllerBase
    {
        private readonly ILogger<ClubsController> _logger;
        private readonly ClubsRepository _clubsRepository;

        public ClubsController(ILogger<ClubsController> logger)
        {
            _logger = logger;
            _clubsRepository = new ClubsRepository("Server=localhost;Database=OpenLeague;Uid=localuser;Pwd=localpass;");
        }

        [HttpGet("clubs")]
        public async Task<GetClubsResponse> Get()
        {
            var clubs = await _clubsRepository.GetClubs();
            return new GetClubsResponse
            {
                Clubs = clubs.Select(clubEntity=>new Club
                {
                    Name = clubEntity.Name,
                    Reference = clubEntity.Reference
                }).ToList()
            };
        }

        [HttpGet("clubs/{clubReference}")]
        public async Task<GetClubResponse> Get(Guid clubReference)
        {
            var club = await _clubsRepository.GetClub(clubReference);
            return new GetClubResponse
            {
                Club = new Club
                {
                    Name = club.Name,
                    Reference = club.Reference
                }
            };
        }
    }
}