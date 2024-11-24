using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using SantApi.Services;
using SantApi.Settings;

namespace SantApi.Tests.Tests.Services.CachedHackerNews
{
    public class CachedHackerNewsServiceFixture
    {
        public Mock<IHttpClientFactory> HttpClientFactoryMock { get; private set; }
        public Mock<HttpMessageHandler> HttpMessageHandlerMock { get; private set; }
        public Mock<IOptions<HackerNewsSettings>> HackerNewsSettingsMock { get; private set; }
        public IMemoryCache MemoryCache { get; private set; }
        public CachedHackerNewsService Service { get; private set; }

        public CachedHackerNewsServiceFixture()
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
                ItemEndpoint = "item/{0}.json",
                CacheExpirationMinutes = 5
            });

            var memoryCacheOptions = new MemoryCacheOptions();
            MemoryCache = new MemoryCache(Options.Create(memoryCacheOptions));

            Service = new CachedHackerNewsService(HttpClientFactoryMock.Object, MemoryCache, HackerNewsSettingsMock.Object);
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
