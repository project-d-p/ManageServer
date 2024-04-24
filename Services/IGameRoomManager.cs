namespace MatchingClient.Services
{
    public interface IGameRoomManager
    {
        void RegisterRoom(string roomId, Dictionary<string, string> roomSettings);
        IEnumerable<Dictionary<string, string>> GetAllRooms(Func<Dictionary<string, string>, bool> filter);
    }
}