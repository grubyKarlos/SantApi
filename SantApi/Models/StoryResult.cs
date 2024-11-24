using System.Text.Json.Serialization;

namespace SantApi.Models
{
    public class StoryResult
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("postedBy")]
        public string PostedBy { get; set; }

        [JsonIgnore]
        public long UnixTime { get; set; }

        [JsonPropertyName("time")]
        public DateTime Time => DateTimeOffset.FromUnixTimeSeconds(UnixTime).UtcDateTime;

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("commentCount")]
        public int CommentCount { get; set; }
    }
}
