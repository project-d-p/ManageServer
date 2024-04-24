using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MatchingClient.Services;
using MatchingClient.Models;

namespace MatchingClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiClientController : ControllerBase
    {
        private readonly RoomMatchManager _roomMatchManager;

        public ApiClientController(RoomMatchManager roomMatchManager)
        {
            _roomMatchManager = roomMatchManager;
        }

        [HttpPost("match")]
        public async Task<IActionResult> MatchPlayers(string player_token)
        {
            try
            {
                await _roomMatchManager.MatchPlayerToRoom(player_token);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }
    }
}
