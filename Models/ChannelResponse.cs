namespace MatchingClient.Models
{
    public class ChannelResponse
    {
        public string ChannelId { get; set; }
        public string UdpIp { get; set; }
        public string UdpPort { get; set; }
        public string TcpIp { get; set; }
        public string TcpPort { get; set; }
    }
}