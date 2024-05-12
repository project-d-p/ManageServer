using Grpc.Core;
using GameEnd; // 프로토콜 버퍼 코드에서 생성된 네임스페이스
using System.Threading.Tasks;
using MatchingClient.Services;
using ManageServer.Data;
using ManageServer.Models;
using Microsoft.EntityFrameworkCore;

namespace MatchingClient.Services
{
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
                    int userId = int.Parse(player.UserId);
                    if (!await ValidatePlayerSummary(player))
                    {
                        Console.WriteLine($"Invalid UserId: {userId}");
                        continue; // 유효하지 않은 UserId는 건너뜁니다.
                    }
                    var playerRecord = new PlayerRecord
                    {
                        UserId = userId, // 유저 ID 파싱 (UserID는 string에서 int로 변환 가정)
                        Rank = (int)player.Rank, // Enum 타입의 Rank를 int로 캐스팅
                        TotalScore = player.TotalScore,
                        CapturedNum = player.CapturedNum,
                        Role = (ManageServer.Models.PlayerRole)player.Role // Enum 타입의 Role을 직접 캐스팅
                    };
                    Console.WriteLine($"PlayerRecord: {playerRecord.UserId}, {playerRecord.Rank}, {playerRecord.TotalScore}, {playerRecord.CapturedNum}, {playerRecord.Role}");
                    _context.PlayerRecords.Add(playerRecord); // PlayerRecord 객체를 컨텍스트에 추가
                }
                
                await _context.SaveChangesAsync(); // 변경사항을 데이터베이스에 비동기적으로 저장
                await ResetRoomLogic(request.RoomId); // 방 리셋 로직 호출 (비동기)
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
        
        private async Task<bool> ValidatePlayerSummary(PlayerSummary playerSummary)
        {
            // UserId를 정수로 변환하고 유효성을 검사합니다.
            if (!int.TryParse(playerSummary.UserId, out int userId))
            {
                Console.WriteLine("Invalid UserId format.");
                return false;
            }
            
            // UserId가 데이터베이스에 존재하는지 확인합니다.
            if (!await _context.Users.AnyAsync(u => u.Id == userId))
            {
                Console.WriteLine($"No user found with UserId: {userId}");
                return false;
            }

            // PlayerRole이 유효한 Enum 값인지 검사합니다.
            if (!Enum.IsDefined(typeof(ManageServer.Models.PlayerRole), (int)playerSummary.Role))
            {
                Console.WriteLine("Invalid player role.");
                return false;
            }

            // TotalScore와 CapturedNum이 음수가 아닌지 검사합니다.
            if (playerSummary.TotalScore < 0 || playerSummary.CapturedNum < 0)
            {
                Console.WriteLine("TotalScore and CapturedNum must be non-negative.");
                return false;
            }

            return true; // 모든 검사를 통과하면 true를 반환합니다.
        }


        private async Task<bool> ResetRoomLogic(string roomId)
        {
            bool result = await _redisCacheManager.ResetRoomAsync(roomId);
            return result;
        }
    }
}