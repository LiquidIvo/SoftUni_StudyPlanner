using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.Quote;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudyPlanner.Services.Core.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly HttpClient _httpClient;

        private record ZenQuoteResponse(string q, string a);

        public QuoteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<QuoteDTO> GetRandomQuoteAsync()
        {
            var response = await _httpClient.GetAsync("https://zenquotes.io/api/random");
            response.EnsureSuccessStatusCode();

            var results = await response.Content
                .ReadFromJsonAsync<List<ZenQuoteResponse>>();

            if (results == null || results.Count == 0)
                throw new Exception("No quote returned from API.");

            return new QuoteDTO
            {
                Text = results[0].q,
                Author = results[0].a
            };



        }
    }
}
