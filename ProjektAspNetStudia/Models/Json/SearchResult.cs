using System.Text.Json.Serialization;

namespace ProjektAspNetStudia.Models.Json
{
    public class SearchResult
    {
        [JsonPropertyName("userid")]
        public string UserId { get; set; }
        [JsonPropertyName("username")]
        public string UserName { get; set; }
    }
}
