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
        public async Task<IActionResult> MatchPlayers([FromBody] PlayerToken player_token)
        {
            Console.WriteLine($"Matching player with token: {player_token.Player_Token}");
            if (player_token.Player_Token == null)
            {
                return BadRequest();
            }
            try
            {
                await _roomMatchManager.MatchPlayerToRoom(player_token.Player_Token);
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
