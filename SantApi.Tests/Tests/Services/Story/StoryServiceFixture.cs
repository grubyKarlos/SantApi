using Moq;
using SantApi.Services;

namespace SantApi.Tests.Tests.Services.Story
{
    public class StoryServiceFixture
    {
        public Mock<ICachedHackerNewsService> CachedHackerNewsServiceMock { get; private set; }
        public StoryService Service { get; private set; }

        public StoryServiceFixture()
        {
            CachedHackerNewsServiceMock = new Mock<ICachedHackerNewsService>();
            Service = new StoryService(CachedHackerNewsServiceMock.Object);
        }

        public void SetupGetBestStoryIdsCachedAsync(List<int> storyIds)
        {
            CachedHackerNewsServiceMock
                .Setup(s => s.GetBestStoryIdsCachedAsync())
                .ReturnsAsync(storyIds);
        }

        public void SetupGetStoryByIdCachedAsync(int storyId, Models.Story? story)
        {
            CachedHackerNewsServiceMock
                .Setup(s => s.GetStoryByIdCachedAsync(storyId))
                .ReturnsAsync(story);
        }
    }
}
