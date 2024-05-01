using MatchingClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MatchingClient.Services
{
    public interface IRedisCacheManager
    {
        Task<List<string>> FindAvailableRoomsAsync();
        Task<Room?> GetRoomByPlayerIdAsync(string playerId);
        Task<Room?> GetRoomByRoomIdAsync(string roomId);
        Task<Room> AddPlayerToRoomAsync(string roomId, string playerId);
        IEnumerable<string> GetAllRoomKeys();
        Task CreateRoomAsync(Room room);
        Task ResetRoomAsync(string roomId);
    }
}