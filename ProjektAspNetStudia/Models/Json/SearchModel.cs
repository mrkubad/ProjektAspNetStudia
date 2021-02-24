using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjektAspNetStudia.Models.Json
{
    public class SearchModel
    {
        [JsonPropertyName("data")]
        public List<SearchResult> Data { get; set; }
    }
}
