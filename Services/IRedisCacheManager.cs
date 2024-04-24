namespace MatchingClient.Services
{
    public interface IRedisCacheManager
    {
        Task<List<string>> FindAvailableRoomsAsync();
        Task<string> FindRoomByPlayerAsync(string playerId);
        Task AddPlayerToRoomAsync(string roomId, string playerId);
        Task CreateRoomAsync(string roomId, string ip, string udpPort, string tcpPort);
        Task RemoveRoomAsync(string roomId);
    }
}