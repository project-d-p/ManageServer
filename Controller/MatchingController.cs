using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Matching;

[ApiController]
[Route("[controller]")]
public class MatchingController : ControllerBase
{
    [HttpPost("match")]
    public async Task<IActionResult> MatchPlayers()
    {
        using var channel = GrpcChannel.ForAddress("http://localhost:5255");
        var client = new MatchingService.MatchingServiceClient(channel);

        var teamPlayers = new TeamPlayers();
        teamPlayers.PlayerIds.Add("player1");
        teamPlayers.PlayerIds.Add("player2");

        var response = await client.SendTeamPlayersAsync(teamPlayers);
        
        return Ok(new {
            UdpIp = response.UdpIp,
            UdpPort = response.UdpPort,
            TcpIp = response.TcpIp,
            TcpPort = response.TcpPort
        });
    }
}
