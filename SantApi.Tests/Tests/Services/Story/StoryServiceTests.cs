namespace SantApi.Tests.Tests.Services.Story
{
    public class StoryServiceTests : IClassFixture<StoryServiceFixture>
    {
        private readonly StoryServiceFixture _fixture;

        public StoryServiceTests(StoryServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetBestStoriesAsync_ReturnsSortedStoriesByScore()
        {
            // Arrange
            var storyIds = new List<int> { 1, 2, 3 };
            var stories = new List<Models.Story?>
            {
                new Models.Story { Title = "Story 1", Score = 50 },
                new Models.Story { Title = "Story 2", Score = 70 },
                new Models.Story { Title = "Story 3", Score = 30 }
            };

            _fixture.SetupGetBestStoryIdsCachedAsync(storyIds);

            for (int i = 0; i < storyIds.Count; i++)
            {
                _fixture.SetupGetStoryByIdCachedAsync(storyIds[i], stories[i]);
            }

            // Act
            var result = await _fixture.Service.GetBestStoriesAsync(3);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Story 2", result[0].Title);
            Assert.Equal("Story 1", result[1].Title);
            Assert.Equal("Story 3", result[2].Title);
        }

        [Fact]
        public async Task GetBestStoriesAsync_SkipsNullStories()
        {
            // Arrange
            var storyIds = new List<int> { 1, 2, 3 };
            var stories = new List<Models.Story?>
            {
                new Models.Story { Title = "Story 1", Score = 50 },
                null,
                new Models.Story { Title = "Story 3", Score = 30 }
            };

            _fixture.SetupGetBestStoryIdsCachedAsync(storyIds);

            for (int i = 0; i < storyIds.Count; i++)
            {
                _fixture.SetupGetStoryByIdCachedAsync(storyIds[i], stories[i]);
            }

            // Act
            var result = await _fixture.Service.GetBestStoriesAsync(3);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Story 1", result[0].Title);
            Assert.Equal("Story 3", result[1].Title);
        }

        [Fact]
        public async Task GetBestStoriesAsync_LimitsResultsToN()
        {
            // Arrange
            var storyIds = new List<int> { 1, 2, 3 };
            var stories = new List<Models.Story?>
            {
                new Models.Story { Title = "Story 1", Score = 50 },
                new Models.Story { Title = "Story 2", Score = 70 },
                new Models.Story { Title = "Story 3", Score = 30 }
            };

            _fixture.SetupGetBestStoryIdsCachedAsync(storyIds);

            for (int i = 0; i < storyIds.Count; i++)
            {
                _fixture.SetupGetStoryByIdCachedAsync(storyIds[i], stories[i]);
            }

            // Act
            var result = await _fixture.Service.GetBestStoriesAsync(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Story 2", result[0].Title);
            Assert.Equal("Story 1", result[1].Title);
        }
    }
}
