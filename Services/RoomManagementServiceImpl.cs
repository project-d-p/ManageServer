using Grpc.Core;
using System.Threading.Tasks;
using Roommanagement; // 프로토콜 버퍼 파일에서 생성된 네임스페이스

namespace MatchingClient.Services
{
    public class RoomManagementServiceImpl : RoomManagementService.RoomManagementServiceBase
    {
        private readonly IRedisCacheManager _redisCacheManager;

        public RoomManagementServiceImpl(IRedisCacheManager redisCacheManager)
        {
            _redisCacheManager = redisCacheManager;
        }

        // Room 정보를 리셋하는 RPC 메소드 구현
        public override async Task<ResetRoomResponse> ResetRoom(ResetRoomRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Resetting room: {request.RoomId}");
            bool success = await ResetRoomLogic(request.RoomId); // 방 리셋 로직 호출

            var response = new ResetRoomResponse
            {
                Success = success // 성공 여부 설정
            };
            return response; // 응답 반환
        }

        // 실제 방 리셋 로직 (예시)
        private async Task<bool> ResetRoomLogic(string roomId)
        {
            bool result = await _redisCacheManager.ResetRoomAsync(roomId);
            return result;
        }
    }
}