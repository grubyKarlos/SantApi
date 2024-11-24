using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using SantApi.Models;
using SantApi.Services;
using SantApi.Settings;

namespace SantApi.Tests.Controllers
{
    public class StoriesControllerTests
    {
        private readonly Mock<IStoryService> _storyServiceMock;
        private readonly StoriesController _controller;
        public Mock<IOptions<HackerNewsSettings>> HackerNewsSettingsMock { get; private set; }

        public StoriesControllerTests()
        {
            _storyServiceMock = new Mock<IStoryService>();
            HackerNewsSettingsMock = new Mock<IOptions<HackerNewsSettings>>();
            HackerNewsSettingsMock.Setup(s => s.Value).Returns(new HackerNewsSettings
            {
                MaxStories = 200,
            });

            _controller = new StoriesController(_storyServiceMock.Object, HackerNewsSettingsMock.Object);
        }

        [Fact]
        public async Task GetBestStories_ReturnsOkResultWithStories()
        {
            // Arrange
            var n = 5;
            var expectedStories = new List<Story>
            {
                new Story { Title = "Story 1", Score = 100 },
                new Story { Title = "Story 2", Score = 80 }
            };

            _storyServiceMock
                .Setup(s => s.GetBestStoriesAsync(n))
                .ReturnsAsync(expectedStories);

            // Act
            var result = await _controller.GetBestStories(n);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var returnedStories = Assert.IsType<List<Story>>(okResult.Value);
            Assert.Equal(expectedStories.Count, returnedStories.Count);
            Assert.Equal(expectedStories[0].Title, returnedStories[0].Title);
        }

        [Fact]
        public async Task GetBestStories_ReturnsBadRequest_WhenNIsInvalid()
        {
            // Arrange
            var n = 0;

            // Act
            var result = await _controller.GetBestStories(n);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task GetBestStories_ReturnsBadRequest_WhenNIsGreaterThanMaxNValue()
        {
            // Arrange
            int invalidN = 201;

            // Act
            var result = await _controller.GetBestStories(invalidN);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Contains("The field 'n' must be between 1 and 200.", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetBestStories_ReturnsOkResult_WhenNIsValid()
        {
            // Arrange
            int validN = 150;
            var expectedStories = new List<Story>
            {
                new Story { Title = "Story 1", Score = 100 },
                new Story { Title = "Story 2", Score = 80 }
            };

            _storyServiceMock
                .Setup(s => s.GetBestStoriesAsync(validN))
                .ReturnsAsync(expectedStories);

            // Act
            var result = await _controller.GetBestStories(validN);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var returnedStories = Assert.IsType<List<Story>>(okResult.Value);
            Assert.Equal(expectedStories.Count, returnedStories.Count);
            Assert.Equal(expectedStories[0].Title, returnedStories[0].Title);
        }
    }
}
