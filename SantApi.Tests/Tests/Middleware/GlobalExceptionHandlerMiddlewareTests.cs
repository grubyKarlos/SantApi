using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SantApi.Middleware;
using System.Text.Json;

namespace SantApi.Tests.Tests.Middleware
{
    public class GlobalExceptionHandlerMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly Mock<ILogger<GlobalExceptionHandlerMiddleware>> _loggerMock;
        private readonly GlobalExceptionHandlerMiddleware _middleware;

        public GlobalExceptionHandlerMiddlewareTests()
        {
            _nextMock = new Mock<RequestDelegate>();
            _loggerMock = new Mock<ILogger<GlobalExceptionHandlerMiddleware>>();
            _middleware = new GlobalExceptionHandlerMiddleware(_nextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task InvokeAsync_NoException_CallsNextMiddleware()
        {
            // Arrange
            var context = new DefaultHttpContext();

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _nextMock.Verify(next => next(context), Times.Once);
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            _nextMock
                .Setup(next => next(It.IsAny<HttpContext>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

            var json = JsonSerializer.Deserialize<JsonElement>(body);
            Assert.Equal("An error occurred while processing your request.", json.GetProperty("message").GetString());
            Assert.Equal("Test exception", json.GetProperty("error").GetString());
        }


        [Fact]
        public async Task InvokeAsync_ExceptionThrown_LogsError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var exception = new Exception("Test exception");
            _nextMock
                .Setup(next => next(It.IsAny<HttpContext>()))
                .ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unhandled exception occurred.")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_ExceptionThrown_SetsContentTypeToJson()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            _nextMock
                .Setup(next => next(It.IsAny<HttpContext>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

            Assert.StartsWith("application/json", context.Response.ContentType);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

            var json = JsonSerializer.Deserialize<JsonElement>(body);
            Assert.Equal("An error occurred while processing your request.", json.GetProperty("message").GetString());
            Assert.Equal("Test exception", json.GetProperty("error").GetString());
        }
    }
}