using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Matching;
using MatchingClient.Models;

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
                UdpIp = response.UDPIP,
                UdpPort = response.UDPPort,
                TcpIp = response.TCPIP,
                TcpPort = response.UDPPort
            });
        }

        private async Task<GameInfo> RequestTeamMatch(string[] playerIds)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5255");
            var client = new MatchingService.MatchingServiceClient(channel);

            var teamPlayers = new TeamPlayers();
            foreach (var playerId in playerIds)
            {
                teamPlayers.PlayerIds.Add(playerId);
            }
            var response = await client.SendTeamPlayersAsync(teamPlayers);
            GameInfo gameInfo = new GameInfo
            {
                UDPIP = response.UdpIp,
                TCPIP = response.TcpIp,
                UDPPort = response.UdpPort,
                TCPPort = response.TcpPort
            };

            return gameInfo;
        }
    }
}
