using StackExchange.Redis;

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

        public async Task<string> FindRoomByPlayerAsync(string playerId)
        {
            var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());
            var roomKeys = server.Keys(pattern: "room:*").ToArray();

            foreach (var key in roomKeys)
            {
                var players = await _db.HashGetAsync(key, "players");
                var playerList = JsonSerializer.Deserialize<List<string>>(players);
                if (playerList.Contains(playerId) && await _db.HashGetAsync(key, "active") == "true")
                {
                    // key.ToString() returns the room id    
                    return key.ToString();
                }
            }

            return null;
        }


        public async Task<Room> AddPlayerToRoomAsync(string roomId, string playerId)
        {
            var room = await _db.HashGetAllAsync(roomId);
            var players = room.FirstOrDefault(x => x.Name == "players").Value;
            var playerList = JsonSerializer.Deserialize<List<string>>(players) ?? new List<string>();
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

        public async Task CreateRoomAsync(Room room)
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

        public async Task RemoveRoomAsync(string roomId)
        {
            await _db.KeyDeleteAsync(roomId);
        }
    }
}