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

        public async Task<string> FindAvailableRoom()
        {
            var availableRooms = await _redisCacheManager.FindAvailableRoomsAsync();
            if (availableRooms != null && availableRooms.Length > 0)
            {
                return availableRooms[0];
            }
            else
            {
                return null;
            }
        }

        public async Task<string> MatchPlayerToRoom(string player_token)
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

        private async Task<string> AddPlayerAndStartGameIfFull(string player_token, string roomId)
        {
            // Add the player to the room
            Room updatedRoom = await _redisCacheManager.AddPlayerToRoomAsync(room, player_token);
            
            // Check if the room is full
            if (updatedRoom.Players.Count >= 3)
            {
                // Start the game
                await _grpcClient.AttachPlayerAsync(new RequestLaunch
                {
                    PlayerToken = updatedRoom.Players,
                    ChannelId = roomId
                });
                return $"Room: {roomId} is full. Game started.";
            }

            // If the room is not full, return a message indicating the player has been added
            return $"Player {player_token} added to room {room}.";
        }


        private async Task<string> CreateNewRoomAndAddPlayer(string player_token)
        {
            Room newRoom = await _grpcClient.CreateChannelAsync();
            await _redisCacheManager.CreateRoomAsync(newRoom);
            await _redisCacheManager.AddPlayerToRoomAsync(newRoom.RoomId, player_token);
            return $"New room created and player {player_token} added to room {newRoom}.";
        }
    }
}
