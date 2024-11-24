using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Moq;
using SantApi.Services;

namespace SantApi.Tests.Integration
{
    public class StoriesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly IConfiguration _configuration;

        public StoriesControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _configuration = factory.Services.GetRequiredService<IConfiguration>();
        }

        [Fact]
        public async Task GetBestStories_ReturnsOk_WhenNIsValid()
        {
            // Arrange
            var n = 2;

            // Act
            var response = await _client.GetAsync($"/api/stories?n={n}");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains("score", result);
        }

        [Fact]
        public async Task GetBestStories_ReturnsBadRequest_WhenNIsInvalid()
        {
            // Arrange
            var invalidN = -1;

            // Act
            var response = await _client.GetAsync($"/api/stories?n={invalidN}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains("The field 'n' must be between", result);
        }

        [Fact]
        public async Task GetBestStories_ReturnsBadRequest_WhenNExceedsMax()
        {
            // Arrange
            //var maxN = 200;
            var maxN = _configuration.GetValue<int>("HackerNewsSettings:MaxStories");
            var n = maxN + 1;

            // Act
            var response = await _client.GetAsync($"/api/stories?n={n}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains($"The field 'n' must be between 1 and {maxN}.", result);
        }

        [Fact]
        public async Task GetBestStories_ReturnsInternalServerError_WhenServiceFails()
        {
            // Arrange
            var mockService = new Mock<IStoryService>();
            mockService
                .Setup(service => service.GetBestStoriesAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Simulated service failure"));

            var factoryWithMock = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(mockService.Object);
                });
            });

            var client = factoryWithMock.CreateClient();

            // Act
            var response = await client.GetAsync("/api/stories?n=5");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GetStoryById_ReturnsNotFound_WhenStoryDoesNotExist()
        {
            // Arrange
            var invalidId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/stories/{invalidId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
