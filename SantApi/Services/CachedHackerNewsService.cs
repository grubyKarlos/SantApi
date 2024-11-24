using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SantApi.Models;
using SantApi.Settings;

namespace SantApi.Services
{
    public class CachedHackerNewsService : HackerNewsService, ICachedHackerNewsService
    {
        private readonly IMemoryCache _cache;
        private readonly HackerNewsSettings _settings;

        public CachedHackerNewsService(IHttpClientFactory httpClientFactory, IMemoryCache cache, IOptions<HackerNewsSettings> settings)
            : base(httpClientFactory, settings)
        {
            _cache = cache;
            _settings = settings.Value;
        }

        public async Task<List<int>> GetBestStoryIdsCachedAsync()
        {
            return await _cache.GetOrCreateAsync("BestStoryIds", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.CacheExpirationMinutes);
                return await GetBestStoryIdsAsync();
            });
        }

        public async Task<Story?> GetStoryByIdCachedAsync(int id)
        {
            return await _cache.GetOrCreateAsync($"Story_{id}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.CacheExpirationMinutes);
                return await GetStoryByIdAsync(id);
            });
        }
    }
}
