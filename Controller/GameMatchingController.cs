using Microsoft.AspNetCore.Mvc;
using MatchingClient.Services;
using MatchingClient.Models;
using ManageServer.Data;

namespace MatchingClient.Controllers
{
    [ApiController]
    [Route("game-matching")]
    public class GameMatchingController : ControllerBase
    {
        private readonly RoomMatchManager _roomMatchManager;

        public GameMatchingController(RoomMatchManager roomMatchManager, ApplicationDbContext context)
        {
            _roomMatchManager = roomMatchManager;
        }

        [HttpPost("match")]
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

        [HttpPost("match-wait")]
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
                    //start Accept Match Timer Here
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
        [HttpPost("agree")]
        public async Task<IActionResult> AcceptStartGame([FromBody] PlayerToken player_token)
        {
            try
            {
                Console.WriteLine($"Accept game for player with token: {player_token.Player_Token}");
                if (string.IsNullOrEmpty(player_token.Player_Token))
                {
                    return BadRequest("Player token is required.");
                }
                
                var AcceptMsg = await _roomMatchManager.AcceptMatch(player_token.Player_Token);
                Console.WriteLine("AcceptMsg: " + AcceptMsg);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while accepting game: {ex.Message}");
                return StatusCode(500, "Internal server error while accepting game.");
            }
        }

        [HttpPost("agree-wait")]
        public async Task<IActionResult> WaitAcceptStartGame([FromBody] PlayerToken player_token, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"Wait for Accept otherPlayers : {player_token.Player_Token}");
                if (string.IsNullOrEmpty(player_token.Player_Token))
                {
                    return BadRequest("Player token is required.");
                }
                var roomInfo = await _roomMatchManager.WaitForAccept(player_token.Player_Token, cancellationToken);
                if (roomInfo == null)
                {
                    return NotFound("fail to GameStart.");
                }
                return Ok(roomInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while waiting for accept: {ex.Message}");
                return StatusCode(500, "Internal server error while waiting for accept.");
            }
        }
    }
}
