using System.Text.Json.Serialization;

namespace MatchingClient.Models
{   
    public class ReMatchInfo
    {
        [JsonPropertyName("player_token")]
        public string? Player_Token { get; set; }
        [JsonPropertyName("room_id")]
        public string? RoomID { get; set; }
    }
}