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
        private readonly GrpcGameServerClient _grpcClient;

        public ApiClientController(GrpcGameServerClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        [HttpPost("match")]
        public async Task<IActionResult> MatchPlayers()
        {
            var response = await _grpcClient.RequestTeamMatch(new string[] { "player1", "player2" });

            return Ok(new {
                UdpIp = response.UDPIP,
                UdpPort = response.UDPPort,
                TcpIp = response.TCPIP,
                TcpPort = response.TCPPort
            });
        }
    }
}
