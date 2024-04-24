using System.Collections.Generic;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;
using MatchingClient.Models;

namespace MatchingClient.Services
{
    public class RoomMatchManager
    {
        private readonly RedisCacheManager _redisCacheManager;
        private readonly GrpcGameServerClient _grpcClient;

        public RoomMatchManager(RedisCacheManager redisCacheManager, GrpcGameServerClient grpcClient)
        {
            _redisCacheManager = redisCacheManager;
            _grpcClient = grpcClient;
        }

        public async Task<string?> FindAvailableRoom()
        {
            try
            {
                List<string> availableRooms = await _redisCacheManager.FindAvailableRoomsAsync();
                if (availableRooms != null && availableRooms.Count() > 0)
                {
                    return availableRooms[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error finding available room: {ex.Message}");
            }
        }

        public async Task<string> MatchPlayerToRoom(string player_token)
        {
            try
            {
                var availableRoom = await FindAvailableRoom();
                if (availableRoom != null)
                {
                    return await AddPlayerAndStartGameIfFull(player_token, availableRoom);
                }
                else
                {
                    return await CreateNewRoomAndAddPlayer(player_token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing the match operation: {ex.Message}");
                return "Error: Failed to process the match operation.";
            }
        }


        private async Task<string> AddPlayerAndStartGameIfFull(string playerToken, string roomId)
        {
            try
            {
                // 플레이어를 방에 추가
                Room updatedRoom = await _redisCacheManager.AddPlayerToRoomAsync(roomId, playerToken);
                
                // 방이 가득 찼는지 확인
                if (updatedRoom.Players.Count >= 3)
                {
                    // 게임 시작
                    var requestLaunch = new Matching.RequestLaunch
                    {
                    ChannelId = roomId
                    };
                    requestLaunch.PlayerToken.AddRange(updatedRoom.Players);  // 리스트에 플레이어 추가
                    await _grpcClient.AttachPlayerAsync(requestLaunch);
                    return $"Room: {roomId} is full. Game started.";
                }

                // 방이 가득 차지 않았다면 플레이어 추가 완료 메시지 반환
                return $"Player {playerToken} added to room {roomId}.";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding player to room: {ex.Message}");
            }
        }


        private async Task<string> CreateNewRoomAndAddPlayer(string player_token)
        {
            try
            {
                Room newRoom = await _grpcClient.CreateChannelAsync();
                await _redisCacheManager.CreateRoomAsync(newRoom);
                await _redisCacheManager.AddPlayerToRoomAsync(newRoom.RoomId, player_token);
                return $"New room created and player {player_token} added to room {newRoom}.";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating new room and adding player: {ex.Message}");
            }
        }
    }
}
