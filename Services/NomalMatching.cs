using MatchingClient.Models;

namespace MatchingClient.Services
{
    public class NomalMatching : MatchingQueue
    {
        private Queue<Player> playerQueue;

        public NomalMatching()
        {
            playerQueue = new Queue<Player>();
        }

        public void EnqueuePlayer(Player player)
        {
            playerQueue.Enqueue(player);
        }
        public Player DequeuePlayer()
        {
            return playerQueue.Dequeue();
        }
        public Team[] FormTeams()
        {
            List<Team> teams = new List<Team>();

            while (playerQueue.Count >= 3)
            {
                Team team = new Team();
                for (int i = 0; i < 3; i++)
                {
                    Player player = playerQueue.Dequeue();
                    team.Players?.Add(player);
                }
                teams.Add(team);
            }

            return teams.ToArray();
        }
    }
}