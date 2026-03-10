using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.Quote;
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
            try
            {
                var response = await _httpClient.GetAsync("https://zenquotes.io/api/random");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var results = JsonSerializer.Deserialize<List<ZenQuoteResponse>>(json);

                if (results != null && results.Count > 0)
                {
                    return new QuoteDTO
                    {
                        Text = results[0].q,
                        Author = results[0].a
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException();
            }
          


            return new QuoteDTO
            {
                Text = "The secret of getting ahead is getting started.",
                Author = "Mark Twain"
            };
        }
    }
}
