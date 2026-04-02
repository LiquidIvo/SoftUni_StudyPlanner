using RichardSzalay.MockHttp;
using StudyPlanner.Services.Core.Services;
using System.Net;

namespace StudyPlanner.Services.Tests
{
    [TestFixture]
    public class QuoteServiceTests
    {
        private MockHttpMessageHandler _mockHttp;
        private QuoteService _service;

        [SetUp]
        public void Setup()
        {
            _mockHttp = new MockHttpMessageHandler();
            var client = _mockHttp.ToHttpClient();
            _service = new QuoteService(client);
        }

        [TearDown]
        public void TearDown()
        {
            _mockHttp.Dispose();
        }

        [Test]
        public async Task GetRandomQuoteAsync_ValidResponse_ReturnsQuoteDTO()
        {
            var json = """[{"q":"The only way to do great work is to love what you do.","a":"Steve Jobs"}]""";

            _mockHttp
                .When("https://zenquotes.io/api/random")
                .Respond("application/json", json);

            var result = await _service.GetRandomQuoteAsync();

            Assert.That(result.Text, Is.EqualTo("The only way to do great work is to love what you do."));
            Assert.That(result.Author, Is.EqualTo("Steve Jobs"));
        }

        [Test]
        public void GetRandomQuoteAsync_ApiReturnsEmptyList_ThrowsHttpRequestException()
        {
            _mockHttp
                .When("https://zenquotes.io/api/random")
                .Respond("application/json", "[]");

            Assert.ThrowsAsync<HttpRequestException>(() => _service.GetRandomQuoteAsync());
        }

        [Test]
        public void GetRandomQuoteAsync_ApiReturnsErrorStatus_ThrowsHttpRequestException()
        {
            _mockHttp
                .When("https://zenquotes.io/api/random")
                .Respond(HttpStatusCode.InternalServerError);

            Assert.ThrowsAsync<HttpRequestException>(() => _service.GetRandomQuoteAsync());
        }

        [Test]
        public void GetRandomQuoteAsync_ApiReturnsInvalidJson_ThrowsException()
        {
            _mockHttp
                .When("https://zenquotes.io/api/random")
                .Respond("application/json", "not valid json");

            Assert.ThrowsAsync<Exception>(() => _service.GetRandomQuoteAsync());
        }
    }
}