using Grpc.Core;
using GameEnd; // 프로토콜 버퍼 코드에서 생성된 네임스페이스
using System.Threading.Tasks;
using MatchingClient.Services;
using ManageServer.Data;
using ManageServer.Models;
using Microsoft.EntityFrameworkCore;
using System;

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

        public override async Task<GameSummaryResponse> GetGameSummary(GameSummaryRequest request, ServerCallContext context)
        {
            var response = new GameSummaryResponse
            {
                Success = true // 성공 여부 설정
            };

            if (!await SaveGameResult(request))
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
                    if (!await ValidatePlayerSummary(player))
                    {
                        Console.WriteLine($"Invalid LoginId: {player.UserId}");
                        continue; // 유효하지 않은 LoginId는 건너뜁니다.
                    }
                    
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.LoginId == player.UserId);
                    if (user == null) continue; // 유저 정보가 없다면 건너뜁니다.
                    
                    var playerRecord = new PlayerRecord
                    {
                        UserId = user.Id, // DB에서 찾은 User ID
                        Rank = (int)player.Rank,
                        TotalScore = player.TotalScore,
                        CapturedNum = player.CapturedNum,
                        Role = (ManageServer.Models.PlayerRole)player.Role
                    };

                    _context.PlayerRecords.Add(playerRecord);
                }
                
                await _context.SaveChangesAsync();
                await ResetRoomLogic(request.RoomId);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool IsValidGameResult(GameSummaryRequest request)
        {
            return request != null && !string.IsNullOrEmpty(request.RoomId) && request.Players != null;
        }
        
        private async Task<bool> ValidatePlayerSummary(PlayerSummary playerSummary)
        {
            // LoginId 검증
            if (string.IsNullOrEmpty(playerSummary.UserId))
            {
                Console.WriteLine("Invalid or missing LoginId.");
                return false;
            }

            // LoginId가 데이터베이스에 존재하는지 확인
            if (!await _context.Users.AnyAsync(u => u.LoginId == playerSummary.UserId))
            {
                Console.WriteLine($"No user found with LoginId: {playerSummary.UserId}");
                return false;
            }

            // PlayerRole 유효성 검사
            if (!Enum.IsDefined(typeof(ManageServer.Models.PlayerRole), (int)playerSummary.Role))
            {
                Console.WriteLine("Invalid player role.");
                return false;
            }

            // 점수 검증
            if (playerSummary.TotalScore < 0 || playerSummary.CapturedNum < 0)
            {
                Console.WriteLine("TotalScore and CapturedNum must be non-negative.");
                return false;
            }

            return true;
        }

        private async Task<bool> ResetRoomLogic(string roomId)
        {
            return await _redisCacheManager.ResetRoomAsync(roomId);
        }
    }
}
