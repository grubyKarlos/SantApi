namespace SantApi.Settings
{
    public class HackerNewsSettings
    {
        public string BaseUrl { get; set; }
        public string BestStoriesEndpoint { get; set; }
        public string ItemEndpoint { get; set; }
        public int CacheExpirationMinutes { get; set; }
        public int MaxStories { get; set; }
    }
}
