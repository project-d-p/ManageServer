using System.Collections.Generic;
using StackExchange.Redis;
using System.Linq;

namespace MatchingClient.Services
{
    public class NormalMatching
    {
        private readonly GameRoomManager _gameRoomManager;
        private readonly GrpcGameServerClient _grpcClient;

        public NormalMatching(GameRoomManager gameRoomManager, GrpcGameServerClient grpcClient)
        {
            _gameRoomManager = gameRoomManager;
            _grpcClient = grpcClient;
        }

        public async Task<Dictionary<string, string>?> FindOrCreateTimeRoomAsync()
        {
            var availableRooms = _gameRoomManager.GetAllRooms(room => room.ContainsKey("status") && room["status"] == "available");
            var firstRoom = availableRooms.FirstOrDefault();
            if (firstRoom != null)
            {
                await _grpcClient.NotifyRoomSelection(firstRoom["roomId"]);
                return firstRoom;
            }

            // If no rooms are available, create a new room
            return await CreateAndRegisterNewRoomAsync();
        }


        private async Task<Dictionary<string, string>?> CreateAndRegisterNewRoomAsync()
        {
            var newRoomSettings = _grpcClient.CreateGameRoom();
            if (newRoomSettings != null)
            {
                _gameRoomManager.RegisterRoom(newRoomSettings["roomId"], newRoomSettings);
                await _grpcClient.NotifyRoomSelection(newRoomSettings["roomId"]);
                return newRoomSettings;
            }

            return null;
        }
    }
}
