using System.Collections.Generic;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;
using MatchingClient.Models;
using System.Threading;

namespace MatchingClient.Services
{
    public class RoomMatchManager
    {
        private readonly IRedisCacheManager _redisCacheManager;
        private readonly GrpcGameServerClient _grpcClient;

        public RoomMatchManager(IRedisCacheManager redisCacheManager, GrpcGameServerClient grpcClient)
        {
            _redisCacheManager = redisCacheManager;
            _grpcClient = grpcClient;
        }
        public async Task<bool> VaildPlayerToken(string player_token)
        {
            var currRooms = _redisCacheManager.GetAllRoomKeys();
            foreach (var room in currRooms)
            {
                Room? currRoom = await _redisCacheManager.GetRoomByRoomIdAsync(room);
                // Add more fields to print if needed
                if (currRoom.Players.Contains(player_token))
                {
                    return false;
                }
            }
            return true;
        }
        public async Task<string?> FindAvailableRoom()
        {
            try
            {
                Console.WriteLine($"Finding available room...");
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
                Console.WriteLine($"MatchPlayerToRoom...");
                if (await VaildPlayerToken(player_token) != true)
                {
                    return "Player already in a room.";
                }
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
        
        public async Task<Room?> WaitForMatch(string playerToken, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"Waiting for match in room: {playerToken}...");
                while (!cancellationToken.IsCancellationRequested)
                {
                    Room? room = await _redisCacheManager.GetRoomByPlayerIdAsync(playerToken);
                    if (room == null)
                    {
                        throw new Exception("Player not found in any room.");
                    }
                    Console.WriteLine($"Room: {room.Players.Count} players in room {room.RoomId}");
                    if (room != null && room.Players.Count >= 3)
                    {
                        // 게임 시작 로직을 여기에 구현
                        return room;
                    }
                    // 필요한 인원이 모이지 않았으면 일정 시간 대기
                    await Task.Delay(5000, cancellationToken);  // 5초마다 검사
                }
                return null;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("The operation was cancelled.");
                throw new OperationCanceledException("The operation was cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while waiting for the match: {ex.Message}");
                throw new Exception($"Error while waiting for the match: {ex.Message}");
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
                Console.WriteLine($"Creating new room and adding player...");
                Room newRoom = await _grpcClient.CreateChannelAsync();
                if (newRoom.RoomId == null)
                {
                    throw new Exception("Error creating new room.");
                }
                await _redisCacheManager.CreateRoomAsync(newRoom);
                await _redisCacheManager.AddPlayerToRoomAsync($"room:{newRoom.RoomId}", player_token);
                return $"New room created and player {player_token} added to room {newRoom}.";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating new room and adding player: {ex.Message}");
            }
        }
    }
}
