using Microsoft.AspNetCore.Mvc;
using StudyPlanner.Services.Core.Contracts;

namespace StudyPlanner.Web.ViewComponents
{
    public class QuoteViewComponent : ViewComponent
    {
        private readonly IQuoteService _quoteService;

        public QuoteViewComponent(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var quote = await _quoteService.GetRandomQuoteAsync();
                return View(quote);
            }
            catch (HttpRequestException ex)
            {
                return Content("Failed to load the quote");
            }
        }
    }
}
