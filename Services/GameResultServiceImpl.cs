using Grpc.Core;
using GameEnd; // 프로토콜 버퍼 코드에서 생성된 네임스페이스
using System.Threading.Tasks;
using MatchingClient.Services;
using ManageServer.Data;
using ManageServer.Models;

public class GameResultServiceImpl : GameResultService.GameResultServiceBase
{
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly ApplicationDbContext _context;

    public GameResultServiceImpl(IRedisCacheManager redisCacheManager, ApplicationDbContext context)
    {
        _redisCacheManager = redisCacheManager;
        _context = context;
    }

    // 게임 요약 정보를 요청하는 메서드 구현
    public override async Task<GameSummaryResponse> GetGameSummary(GameSummaryRequest request, ServerCallContext context)
    {
        var response = new GameSummaryResponse
        {
            Success = true // 성공 여부 설정
        };

        if ( await SaveGameResult(request) == false)
        {
            response.Success = false;
        }

        return response;
    }

    private async Task<bool> SaveGameResult(GameSummaryRequest request)
    {
        if (!IsValidGameResult(request))
        {
            return false;
        }
               try
        {
            foreach (var player in request.Players)
            {
                var playerRecord = new PlayerRecord
                {
                    UserId = int.Parse(player.UserId), // 유저 ID 파싱 (UserID는 string에서 int로 변환 가정)
                    Rank = (int)player.Rank, // Enum 타입의 Rank를 int로 캐스팅
                    TotalScore = player.TotalScore,
                    CapturedNum = player.CapturedNum,
                    Role = (ManageServer.Models.PlayerRole)player.Role // Enum 타입의 Role을 직접 캐스팅
                };

                _context.PlayerRecords.Add(playerRecord); // PlayerRecord 객체를 컨텍스트에 추가
            }
            
            await _context.SaveChangesAsync(); // 변경사항을 데이터베이스에 비동기적으로 저장
        }
        catch
        {
            return false; // 예외 발생 시, false 반환
        }
        return true;
    }

    private bool IsValidGameResult(GameSummaryRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.RoomId) || string.IsNullOrEmpty(request.Players.ToString()))
        {
            return false;
        }
        return true;
    }
}
