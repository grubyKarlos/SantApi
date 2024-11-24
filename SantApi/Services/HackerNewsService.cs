using Microsoft.Extensions.Options;
using SantApi.Models;
using SantApi.Settings;

namespace SantApi.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly HackerNewsSettings _settings;

        public HackerNewsService(IHttpClientFactory httpClientFactory, IOptions<HackerNewsSettings> settings)
        {
            _httpClient = httpClientFactory.CreateClient(AppConstants.HttpClientName);
            _settings = settings.Value;
        }

        public async Task<List<int>> GetBestStoryIdsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<int>>(_settings.BestStoriesEndpoint);
            return response ?? new List<int>();
        }

        public async Task<Story?> GetStoryByIdAsync(int id)
        {
            var url = string.Format(_settings.ItemEndpoint, id);
            return await _httpClient.GetFromJsonAsync<Story>(url);
        }
    }
}
