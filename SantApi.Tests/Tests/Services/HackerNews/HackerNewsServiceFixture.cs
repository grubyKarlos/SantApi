using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using SantApi.Services;
using SantApi.Settings;

namespace SantApi.Tests.Tests.Services.HackerNews
{
    public class HackerNewsServiceFixture
    {
        public Mock<IHttpClientFactory> HttpClientFactoryMock { get; private set; }
        public Mock<HttpMessageHandler> HttpMessageHandlerMock { get; private set; }
        public Mock<IOptions<HackerNewsSettings>> HackerNewsSettingsMock { get; private set; }
        public HackerNewsService Service { get; private set; }

        public HackerNewsServiceFixture()
        {
            HttpMessageHandlerMock = new Mock<HttpMessageHandler>();

            var httpClient = new HttpClient(HttpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://test-url/")
            };

            HttpClientFactoryMock = new Mock<IHttpClientFactory>();
            HttpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            HackerNewsSettingsMock = new Mock<IOptions<HackerNewsSettings>>();
            HackerNewsSettingsMock.Setup(s => s.Value).Returns(new HackerNewsSettings
            {
                BestStoriesEndpoint = "beststories.json",
                ItemEndpoint = "item/{0}.json"
            });

            Service = new HackerNewsService(HttpClientFactoryMock.Object, HackerNewsSettingsMock.Object);
        }

        public void SetupHttpMessageHandler(string endpoint, string jsonResponse)
        {
            HttpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().EndsWith(endpoint)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });
        }
    }
}
