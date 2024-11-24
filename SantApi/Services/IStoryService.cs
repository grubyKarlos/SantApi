using SantApi.Models;

namespace SantApi.Services
{
    public interface IStoryService
    {
        Task<List<Story>> GetBestStoriesAsync(int n);
    }
}
