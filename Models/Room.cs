using System.Collections.Generic;

namespace MatchingClient.Models
{
    public class Room
    {
        public string? RoomId { get; set; }
        public List<string> Players { get; set; }
        public bool IsActive { get; set; }
        public string? IP { get; set; }
        public string? UdpPort { get; set; }
        public string? TcpPort { get; set; }

        public Room()
        {
            Players = new List<string>();
        }

        public Room(string roomId, string ip, string udpPort, string tcpPort, bool isActive = false)
        {
            RoomId = roomId;
            IP = ip;
            UdpPort = udpPort;
            TcpPort = tcpPort;
            IsActive = isActive;
            Players = new List<string>();
        }
    }
}
