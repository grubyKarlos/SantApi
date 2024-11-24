using SantApi.Models;

namespace SantApi.Services
{
    public class StoryService : IStoryService
    {
        private readonly ICachedHackerNewsService _cachedHackerNewsService;

        public StoryService(ICachedHackerNewsService cachedHackerNewsService)
        {
            _cachedHackerNewsService = cachedHackerNewsService;
        }

        public async Task<List<StoryResult>> GetBestStoriesAsync(int n)
        {
            var ids = await _cachedHackerNewsService.GetBestStoryIdsCachedAsync();

            var tasks = ids.Take(n).Select(_cachedHackerNewsService.GetStoryByIdCachedAsync);
            var stories = await Task.WhenAll(tasks);

            var sortedStories = stories
                .Where(s => s != null)
                .OrderByDescending(s => s!.Score)
                .Select(apiStory => new StoryResult
                {
                    Title = apiStory.Title,
                    Uri = apiStory.Uri,
                    PostedBy = apiStory.PostedBy,
                    UnixTime = apiStory.UnixTime,
                    Score = apiStory.Score,
                    CommentCount = apiStory.CommentCount
                })
                .ToList();

            return sortedStories;
        }
    }
}
