using System.Text.Json.Serialization;

namespace MatchingClient.Models
{   
    public class PlayerToken
    {
        [JsonPropertyName("player_token")]
        public string? Player_Token { get; set; }
    }
}
