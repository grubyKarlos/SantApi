using SantApi.Models;
using System.Text.Json;

namespace SantApi.Tests.Tests.Services.HackerNews
{
    public class HackerNewsServiceTests : IClassFixture<HackerNewsServiceFixture>
    {
        private readonly HackerNewsServiceFixture _fixture;

        public HackerNewsServiceTests(HackerNewsServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetBestStoryIdsAsync_ReturnsListOfIds()
        {
            // Arrange
            var expectedIds = new List<int> { 1, 2, 3 };
            var jsonResponse = JsonSerializer.Serialize(expectedIds);

            _fixture.SetupHttpMessageHandler("beststories.json", jsonResponse);

            // Act
            var result = await _fixture.Service.GetBestStoryIdsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedIds, result);
        }

        [Fact]
        public async Task GetStoryByIdAsync_ReturnsStoryDetails()
        {
            // Arrange
            var storyId = 123;
            var expectedStory = new Models.Story
            {
                Title = "Test Story",
                Uri = "http://example.com",
                PostedBy = "user",
                Score = 100,
                CommentCount = 10
            };
            var jsonResponse = JsonSerializer.Serialize(expectedStory);

            _fixture.SetupHttpMessageHandler($"{storyId}.json", jsonResponse);

            // Act
            var result = await _fixture.Service.GetStoryByIdAsync(storyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedStory.Title, result.Title);
            Assert.Equal(expectedStory.Score, result.Score);
        }
    }
}
