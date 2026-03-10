using StudyPlanner.Services.Core.Models.Quote;

namespace StudyPlanner.Services.Core.Contracts
{
    public interface IQuoteService
    {
        Task<QuoteDTO> GetRandomQuoteAsync();
    }
}
