using Microsoft.AspNetCore.Mvc;
using ManageServer.Data;
using Microsoft.EntityFrameworkCore;

namespace MatchingClient.Controllers
{
    [ApiController]
    [Route("player-records")]
    public class PlayerRecordsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlayerRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetPlayerRecords(int userId)
        {
            var playerRecords = await _context.PlayerRecords
                .Where(pr => pr.UserId == userId)
                .Select(pr => new {
                    RecordId = pr.RecordId,
                    UserId = pr.UserId,
                    Rank = pr.Rank,
                    TotalScore = pr.TotalScore,
                    CapturedNum = pr.CapturedNum,
                    Role = pr.Role.ToString()  // Enum 값을 문자열로 변환
                })
                .ToListAsync();
            
            if (playerRecords == null || playerRecords.Count == 0)
            {
                return NotFound($"No records found for user ID: {userId}");
            }
            
            return Ok(playerRecords);
        }
    }
}
