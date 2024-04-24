using StackExchange.Redis;

namespace MatchingClient.Services
{
    public class GameRoomManager: IGameRoomManager
    {
        private readonly IDatabase _db;

        public GameRoomManager(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public void RegisterRoom(string roomId, Dictionary<string, string> roomSettings)
        {
            var hashEntries = new List<HashEntry>();
            foreach (var setting in roomSettings)
            {
                hashEntries.Add(new HashEntry(setting.Key, setting.Value));
            }

            _db.HashSet("room:" + roomId, hashEntries.ToArray());
        }

        public IEnumerable<Dictionary<string, string>> GetAllRooms(Func<Dictionary<string, string>, bool> filter)
        {
            var server = ConnectionMultiplexer.Connect("localhost").GetServer("localhost", 6379);
            var keys = server.Keys(pattern: "room:*");
            foreach (var key in keys)
            {
                var hashEntries = _db.HashGetAll(key);
                var roomInfo = new Dictionary<string, string>();
                foreach (var entry in hashEntries)
                {
                    roomInfo[entry.Name.ToString()!] = entry.Value.ToString()!;
                }

                if (filter(roomInfo))
                {
                    yield return roomInfo;
                }
            }
        }
    }
}