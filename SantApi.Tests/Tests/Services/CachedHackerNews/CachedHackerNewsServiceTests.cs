using Moq;
using Moq.Protected;
using System.Text.Json;

namespace SantApi.Tests.Tests.Services.CachedHackerNews
{
    public class CachedHackerNewsServiceTests : IClassFixture<CachedHackerNewsServiceFixture>
    {
        private readonly CachedHackerNewsServiceFixture _fixture;

        public CachedHackerNewsServiceTests(CachedHackerNewsServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetBestStoryIdsCachedAsync_ReturnsListOfIdsFromCache()
        {
            // Arrange
            var expectedIds = new List<int> { 1, 2, 3 };
            var jsonResponse = JsonSerializer.Serialize(expectedIds);

            _fixture.SetupHttpMessageHandler("beststories.json", jsonResponse);

            // Act
            var firstCallResult = await _fixture.Service.GetBestStoryIdsCachedAsync();
            var secondCallResult = await _fixture.Service.GetBestStoryIdsCachedAsync();

            // Assert
            Assert.NotNull(firstCallResult);
            Assert.Equal(expectedIds, firstCallResult);

            Assert.NotNull(secondCallResult);
            Assert.Equal(expectedIds, secondCallResult);

            _fixture.HttpMessageHandlerMock
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(
                        req => req.RequestUri.ToString().EndsWith("beststories.json")
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );
        }


        [Fact]
        public async Task GetStoryByIdCachedAsync_ReturnsStoryDetailsFromCache()
        {
            // Arrange
            var storyId = 123;
            var expectedStory = new Models.Story
            {
                Title = "Cached Story",
                Uri = "http://example.com",
                PostedBy = "user",
                Score = 200,
                CommentCount = 50
            };
            var jsonResponse = JsonSerializer.Serialize(expectedStory);

            _fixture.SetupHttpMessageHandler($"{storyId}.json", jsonResponse);

            // Act
            var firstCallResult = await _fixture.Service.GetStoryByIdCachedAsync(storyId);
            var secondCallResult = await _fixture.Service.GetStoryByIdCachedAsync(storyId);

            // Assert
            Assert.NotNull(firstCallResult);
            Assert.Equal(expectedStory.Title, firstCallResult.Title);

            Assert.NotNull(secondCallResult);
            Assert.Equal(expectedStory.Title, secondCallResult.Title);

            _fixture.HttpMessageHandlerMock
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                );
        }
    }
}
