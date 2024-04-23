using Microsoft.AspNetCore.Mvc;
using Grpc.Net.Client;
using Matching;

namespace MatchingClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MatchingController : ControllerBase
    {
        [HttpPost("match")]
        public async Task<IActionResult> MatchPlayers()
        {
            var response = await RequestTeamMatch(new string[] { "player1", "player2" });

            return Ok(new {
                UdpIp = response.UdpIp,
                UdpPort = response.UdpPort,
                TcpIp = response.TcpIp,
                TcpPort = response.TcpPort
            });
        }

        private async Task<TeamMatchResponse> RequestTeamMatch(string[] playerIds)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5255");
            var client = new MatchingService.MatchingServiceClient(channel);

            var teamPlayers = new TeamPlayers();
            foreach (var playerId in playerIds)
            {
                teamPlayers.PlayerIds.Add(playerId);
            }

            return await client.SendTeamPlayersAsync(teamPlayers);
        }
    }
}
