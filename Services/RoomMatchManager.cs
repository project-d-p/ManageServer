using System.Collections.Generic;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;
using MatchingClient.Models;
using System.Threading;
using Matching;

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
                if (currRoom == null)
                {
                    throw new Exception("Error getting room.");
                }
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
        public async Task<string?> WaitForMatch(string playerToken, CancellationToken cancellationToken)
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
                    if (room != null && room.Players.Count >= 4)
                    {
                        return "ready to start";
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
        public async Task<string> AcceptMatch(string playerToken)
        {
            try
            {
                //이미 게임을 허락한 유저에 대한 예외처리 필요
                Room? room = await _redisCacheManager.GetRoomByPlayerIdAsync(playerToken);
                if (room == null)
                {
                    throw new Exception("Player not found in any room.");
                }
                Room updatedRoom = await _redisCacheManager.AddPlayerToFieldAsync(room.RoomId, playerToken, "accept_players", true);
                if (updatedRoom.AcceptPlayers.Count >= 4)
                {
                    Console.WriteLine($"Room: {updatedRoom.RoomId} is full. Game started.");
                    // 게임 시작
                    if (updatedRoom.RoomId == null)
                    {
                        throw new Exception("Error getting room ID.");
                    }
                    var requestLaunch = new Matching.RequestLaunch
                    {
                        ChannelId = updatedRoom.RoomId.Replace("room:", "")
                    };
                    requestLaunch.PlayerToken.AddRange(updatedRoom.AcceptPlayers);  // 리스트에 플레이어 추가
                    await _grpcClient.AttachPlayerAsync(requestLaunch);
                    return $"Room: {updatedRoom.RoomId}. Game started.";
                }
                return $"Player {playerToken} accept to start game: {updatedRoom.RoomId}.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while waiting for the match: {ex.Message}");
                throw new Exception($"Error while waiting for the match: {ex.Message}");
            }
        }
        public async Task<Room?> WaitForAccept(string playerToken, CancellationToken cancellationToken)
        {
            try
            {
                int count = 0;
                while (!cancellationToken.IsCancellationRequested)
                {
                    Room? room = await _redisCacheManager.GetRoomByPlayerIdAsync(playerToken);
                    if (room == null)
                    {
                        throw new Exception("Player not found in any room.");
                    }
                    Console.WriteLine($"Room: {room.AcceptPlayers.Count} players in room {room.RoomId}");
                    if (room != null && room.AcceptPlayers.Count >= 4)
                    {
                        return room;
                    }
                    // 필요한 인원이 모이지 않았으면 일정 시간 대기
                    await Task.Delay(1000, cancellationToken);  // 1초마다 검사

                    count++;
                    if (room != null && count >= 15)
                    {
                        Room? updateRoom = await _redisCacheManager.GetRoomByPlayerIdAsync(playerToken);
                        if (updateRoom == null)
                        {
                            throw new Exception("Player not found in any room.");
                        }
                        if (updateRoom.Players.Count == 3 && updateRoom.RoomId != null)
                            await _redisCacheManager.ResetRoomAsync(updateRoom.RoomId, new string[] { "accept_players", "players" });
                        await _redisCacheManager.AddPlayerToFieldAsync(updateRoom.RoomId, playerToken, "players", true);
                        return null;
                    }
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
                Room updatedRoom = await _redisCacheManager.AddPlayerToFieldAsync(roomId, playerToken, "players", false);
                
                // 방이 가득 찼는지 확인
                if (updatedRoom.Players.Count >= 4)
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
                await _redisCacheManager.AddPlayerToFieldAsync($"room:{newRoom.RoomId}", player_token, "players", false);
                return $"New room created and player {player_token} added to room {newRoom}.";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating new room and adding player: {ex.Message}");
            }
        }
        public async Task<string> AcceptReMatch(string roomId, string playerToken)
        {
            try
            {
                Room updatedRoom = await _redisCacheManager.AddPlayerToFieldAsync(roomId, playerToken, "players", true);
                if (updatedRoom.RoomId != null && updatedRoom.Players.Count >= 4)
                {
                    var requestLaunch = new Matching.RequestLaunch
                    {
                        ChannelId = updatedRoom.RoomId.Replace("room:", "")
                    };
                    requestLaunch.PlayerToken.AddRange(updatedRoom.Players);  // 리스트에 플레이어 추가
                    Room newRoom = await _grpcClient.ReplayMatchAsync(requestLaunch);
                    newRoom.Players = updatedRoom.Players;
                    newRoom.AcceptPlayers = updatedRoom.AcceptPlayers;
                    await _redisCacheManager.CreateRoomAsync(newRoom);
                    updatedRoom = newRoom;
                }
                updatedRoom = await _redisCacheManager.AddPlayerToFieldAsync(roomId, playerToken, "accept_players", true);
                if (updatedRoom.AcceptPlayers.Count >= 4)
                {
                    Console.WriteLine($"Room: {updatedRoom.RoomId} is full. Game started.");
                    return $"Room: {updatedRoom.RoomId}. Game started.";
                }
                return $"Player {playerToken} accept to restart game: {updatedRoom.RoomId}.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while waiting for the match: {ex.Message}");
                throw new Exception($"Error while waiting for the match: {ex.Message}");
            }
        }
        public async Task<Room?> WaitForAcceptReMatch(string playerToken, CancellationToken cancellationToken)
        {
            try
            {
                int count = 0;
                while (!cancellationToken.IsCancellationRequested)
                {
                    Room? room = await _redisCacheManager.GetRoomByPlayerIdAsync(playerToken);
                    if (room == null)
                    {
                        throw new Exception("Player not found in any room.");
                    }
                    Console.WriteLine($"Room: {room.AcceptPlayers.Count} players in room {room.RoomId}");
                    if (room != null && room.AcceptPlayers.Count >= 4)
                    {
                        return room;
                    }
                    // 필요한 인원이 모이지 않았으면 일정 시간 대기
                    await Task.Delay(1000, cancellationToken);  // 1초마다 검사

                    count++;
                    if (room != null && count >= 15)
                    {
                        Room? updateRoom = await _redisCacheManager.GetRoomByPlayerIdAsync(playerToken);
                        if (updateRoom == null || updateRoom.RoomId == null)
                        {
                            throw new Exception("Player not found in any room.");
                        }
                        await _redisCacheManager.ResetRoomAsync(updateRoom.RoomId);
                        return null;
                    }
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
    }
}
