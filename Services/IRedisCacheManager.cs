using MatchingClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace MatchingClient.Services
{
    public interface IRedisCacheManager
    {
        Task<List<string>> FindAvailableRoomsAsync();
        Task<string?> FindRoomByPlayerAsync(string playerId);
        Task<Room> AddPlayerToRoomAsync(string roomId, string playerId);
        Task CreateRoomAsync(Room room);
        Task RemoveRoomAsync(string roomId);
    }
}