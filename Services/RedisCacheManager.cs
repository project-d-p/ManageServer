using StackExchange.Redis;
using MatchingClient.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace MatchingClient.Services
{
    public class RedisCacheManager : IRedisCacheManager
    {
        private readonly IDatabase _db;

        public RedisCacheManager(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }
        // Redis 락 관련 메서드
        public async Task<bool> AcquireLockAsync(string lockKey, TimeSpan expiry, bool waitForLock)
        {
            if (waitForLock)
            {
                int maxAttempts = 3;
                int attempts = 0;
                while (true)
                {
                    bool acquired = await _db.StringSetAsync(lockKey, "waiting", expiry, When.NotExists);
                    if (acquired)
                    {
                        return true;
                    }
                    attempts++;
                    if (attempts >= maxAttempts)
                    {
                        throw new Exception($"Failed to acquire lock after {maxAttempts} attempts.");
                    }
                    await Task.Delay(1000); // 일정 시간 대기 후 다시 시도
                }
            }
            else
            {
                // 기존 락 획득 방식 사용
                bool acquired = await _db.StringSetAsync(lockKey, "locked", expiry, When.NotExists);
                return acquired;
            }
        }

        public async Task ReleaseLockAsync(string lockKey)
        {
            await _db.KeyDeleteAsync(lockKey);
        }
        // Redis 관련 공용 메서드
        public async Task<Room?> GetRoomByRoomIdAsync(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                throw new ArgumentNullException(nameof(roomId), "Room ID cannot be null or empty.");
            }
            HashEntry[] hashEntries = await _db.HashGetAllAsync(roomId);
            if (hashEntries.Length == 0)
            {
                throw new KeyNotFoundException($"No room found with ID: {roomId}");
            }
            var room = new Room
            {
                RoomId = roomId
            };

            foreach (var entry in hashEntries)
            {
                switch (entry.Name.ToString())
                {
                    case "players":
                        string playersJson = entry.Value.HasValue ? entry.Value.ToString() : "[]";
                        room.Players = JsonSerializer.Deserialize<List<string>>(playersJson) ?? new List<string>();
                        break;
                    case "active":
                        if (entry.Value.HasValue)
                            room.IsActive = bool.TryParse(entry.Value, out bool isActive);
                        else
                            room.IsActive = false;
                        break;
                    case "ip":
                        room.IP = entry.Value;
                        break;
                    case "udpPort":
                        room.UdpPort = entry.Value;
                        break;
                    case "tcpPort":
                        room.TcpPort = entry.Value;
                        break;
                }
            }
            return room;
        }
        public async Task<Room?> GetRoomByPlayerIdAsync(string playerId)
        {
            if (string.IsNullOrEmpty(playerId))
            {
                throw new ArgumentNullException(nameof(playerId), "Player ID cannot be null or empty.");
            }
            try
            {
                var roomKeys = GetAllRoomKeys();

                foreach (var key in roomKeys)
                {
                    var room = await GetRoomByRoomIdAsync(key);

                    if (room != null && room.Players.Contains(playerId))
                    {
                        return room;
                    }
                }
                return null; // No room found with the specified player ID
            }
            catch (Exception ex)
            {
                throw new Exception("Error accessing Redis cache.", ex);
            }
        }
        public IEnumerable<string> GetAllRoomKeys()
        {
            var endpoint = _db.Multiplexer.GetEndPoints()[0];
            var server = _db.Multiplexer.GetServer(endpoint);
            var keys = new List<string>();

            // Keys 메서드를 사용하여 모든 'room:*' 키를 검색합니다.
            // 이 메소드는 내부적으로 SCAN을 사용하며, 모든 키를 적절히 가져옵니다.
            foreach (var key in server.Keys(pattern: "room:*", pageSize: 250))
            {
                keys.Add(key.ToString());
            }

            return keys;
        }
        // Matching 관련
        public async Task<List<string>> FindAvailableRoomsAsync()
        {
            try
            {
                var roomKeys = GetAllRoomKeys();
                var availableRooms = new List<string>();

                foreach (var key in roomKeys)
                {
                    
                    var isActive = await _db.HashGetAsync(key, "active");
                    if (isActive == "False")
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
        public async Task<Room> AddPlayerToFieldAsync(string? roomId, string? playerId, string fieldName, bool waitForLock)
        {
            if (roomId == null || playerId == null)
            {
                throw new ArgumentNullException("roomId or playerId", "Room ID and Player ID must not be null.");
            }
            string lockKey = $"lock:{roomId}";
            TimeSpan lockTimeout = TimeSpan.FromSeconds(5); // 예: 10초 동안 락 유지
            // 락 획득 시도
            bool lockAcquired = await AcquireLockAsync(lockKey, lockTimeout, waitForLock);
            if (!lockAcquired)
            {
                throw new Exception("Unable to acquire lock for room operations.");
            }
            try
            {
                Room? room = await GetRoomByRoomIdAsync(roomId);
                if (room == null)
                {
                    throw new KeyNotFoundException($"No room found with ID: {roomId}");
                }

                // 여기서 fieldName을 사용하여 필드를 선택합니다.
                List<string>? fieldPlayers = fieldName switch
                {
                    "players" => room.Players,
                    "accept_players" => room.AcceptPlayers,
                    _ => throw new ArgumentException("Invalid field name.", nameof(fieldName))
                };

                if (!fieldPlayers.Contains(playerId))
                {
                    fieldPlayers.Add(playerId);
                    await _db.HashSetAsync(roomId, fieldName, JsonSerializer.Serialize(fieldPlayers));
                }
                return room;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error accessing Redis cache.AddPlayer to {fieldName}", ex);
            }
            finally
            {
                // 작업 완료 후 락 해제
                await ReleaseLockAsync(lockKey);
            }
        }

        public async Task CreateRoomAsync(Room room)
        {
            try
            {
                var hashEntries = new HashEntry[]
                {
                    new HashEntry("players", JsonSerializer.Serialize(room.Players)),
                    new HashEntry("accept_players", JsonSerializer.Serialize(room.AcceptPlayers)),
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
        public async Task<bool> ResetRoomAsync(string roomId)
        {
            var roomKey = $"room:{roomId}";
            var fields = new RedisValue[] {"players", "accpet_players", "active", "ip", "udpPort", "tcpPort"};
            try
            {
                // 모든 필드를 초기화합니다 (값을 null로 설정)
                foreach (var field in fields)
                {
                    await _db.HashSetAsync(roomKey, field, RedisValue.Null);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error accessing Redis cache.", ex);
            }
        }
    }
}