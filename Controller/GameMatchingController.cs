using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MatchingClient.Services;
using MatchingClient.Models;
using System.Threading; // CancellationToken 사용을 위해 추가

namespace MatchingClient.Controllers
{
    [ApiController]
    [Route("game-matching")]
    public class GameMatchingController : ControllerBase
    {
        private readonly RoomMatchManager _roomMatchManager;

        public GameMatchingController(RoomMatchManager roomMatchManager)
        {
            _roomMatchManager = roomMatchManager;
        }

        [HttpPost("request-match")]
        public async Task<IActionResult> MatchPlayers([FromBody] PlayerToken player_token)
        {
            Console.WriteLine($"Matching player with token: {player_token.Player_Token}");
            if (player_token.Player_Token == null)
            {
                return BadRequest("Player token is required.");
            }
            try
            {
                await _roomMatchManager.MatchPlayerToRoom(player_token.Player_Token);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("status")]
        public async Task<IActionResult> WaitForMatch([FromBody] PlayerToken player_token, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Waiting for a match for player with token: {player_token.Player_Token}");
            if (string.IsNullOrEmpty(player_token.Player_Token))
            {
                return BadRequest("Player token is required.");
            }
            try
            {
                var matchFound = await _roomMatchManager.WaitForMatch(player_token.Player_Token, cancellationToken);
                if (matchFound != null)
                {
                    return Ok(matchFound);
                }
                else
                {
                    return NotFound("No match found.");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Request was canceled.");
                return BadRequest("Request was canceled by the client.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while waiting for match: {ex.Message}");
                return StatusCode(500, "Internal server error while waiting for match.");
            }
        }
    }
}
