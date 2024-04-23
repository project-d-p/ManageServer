using MatchingClient.Models;

namespace MatchingClient.Services
{
    public interface MatchingQueue
    {
        void EnqueuePlayer(Player player);
        Player DequeuePlayer();
    }
}