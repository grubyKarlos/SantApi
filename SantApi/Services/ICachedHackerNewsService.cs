using SantApi.Models;

namespace SantApi.Services
{
    public interface ICachedHackerNewsService : IHackerNewsService
    {
        Task<List<int>> GetBestStoryIdsCachedAsync();
        Task<Story?> GetStoryByIdCachedAsync(int id);
    }
}
