using StackExchange.Redis;
using MatchingClient.Models;
using System.Text.Json;

namespace MatchingClient.Services
{
    public class RedisCacheManager : IRedisCacheManager
    {
        private readonly IDatabase _db;

        public RedisCacheManager(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<List<string>> FindAvailableRoomsAsync()
        {
            try
            {
                var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());
                var roomKeys = server.Keys(pattern: "room:*").ToArray();
                var availableRooms = new List<string>();

                foreach (var key in roomKeys)
                {
                    var isActive = await _db.HashGetAsync(key, "active");
                    if (isActive == "false")
                    {
                        availableRooms.Add(key.ToString());
                    }
                }

                return availableRooms;
            }
            catch (Exception ex)
            {
                throw new Exception("Error accessing Redis cache.", ex);
            }
        }

        public async Task<string?> FindRoomByPlayerAsync(string playerId)
        {
            if (playerId == null)
            {
                throw new ArgumentNullException(nameof(playerId));
            }

            try
            {
                var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());
                var roomKeys = server.Keys(pattern: "room:*").ToArray();

                foreach (var key in roomKeys)
                {
                    string? players = await _db.HashGetAsync(key, "players");
                    List<string>? playerList = null;
                    if (players != null)
                    {   
                        playerList = JsonSerializer.Deserialize<List<string>>(players);
                    }
                    if (playerList != null && playerList?.Contains(playerId) == true && await _db.HashGetAsync(key, "active") == "true")
                    {
                        // key.ToString() returns the room id    
                        return key.ToString();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error accessing Redis cache.", ex);
            }
        }

        public async Task<Room> AddPlayerToRoomAsync(string? roomId, string? playerId)
        {
            if (roomId == null || playerId == null)
            {
                throw new ArgumentNullException(nameof(roomId));
            }

            try
            {
                var room = await _db.HashGetAllAsync(roomId);
                string? players = room.FirstOrDefault(x => x.Name == "players").Value;
                List<string> playerList = new List<string>(); 
                if (players != null)
                    playerList = JsonSerializer.Deserialize<List<string>>(players) ?? new List<string>();
                playerList.Add(playerId);
                await _db.HashSetAsync(roomId, "players", JsonSerializer.Serialize(playerList));

                return new Room
                {
                    RoomId = roomId,
                    Players = playerList,
                    IsActive = false,
                    IP = room.FirstOrDefault(x => x.Name == "ip").Value,
                    UdpPort = room.FirstOrDefault(x => x.Name == "udpPort").Value,
                    TcpPort = room.FirstOrDefault(x => x.Name == "tcpPort").Value
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error accessing Redis cache.", ex);
            }
        }

        public async Task CreateRoomAsync(Room room)
        {
            try
            {
                var hashEntries = new HashEntry[]
                {
                    new HashEntry("players", JsonSerializer.Serialize(room.Players)),
                    new HashEntry("active", room.IsActive.ToString()),
                    new HashEntry("ip", room.IP),
                    new HashEntry("udpPort", room.UdpPort),
                    new HashEntry("tcpPort", room.TcpPort)
                };

                await _db.HashSetAsync($"room:{room.RoomId}", hashEntries);
            }
            catch (Exception ex)
            {
                throw new Exception("Error accessing Redis cache.", ex);
            }
        }

        public async Task RemoveRoomAsync(string roomId)
        {
            try
            {
                await _db.KeyDeleteAsync(roomId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error accessing Redis cache.", ex);
            }
        }
    }
}