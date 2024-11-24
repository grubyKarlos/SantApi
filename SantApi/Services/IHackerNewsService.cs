using SantApi.Models;

namespace SantApi.Services
{
    public interface IHackerNewsService
    {
        Task<List<int>> GetBestStoryIdsAsync();
        Task<Story?> GetStoryByIdAsync(int id);
    }
}
