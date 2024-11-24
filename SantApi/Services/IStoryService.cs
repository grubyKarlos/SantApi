using SantApi.Models;

namespace SantApi.Services
{
    public interface IStoryService
    {
        Task<List<StoryResult>> GetBestStoriesAsync(int n);
    }
}
