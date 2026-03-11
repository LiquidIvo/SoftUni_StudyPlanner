using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyPlanner.Services.Core.Contracts;

namespace StudyPlanner.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        private readonly IQuoteService _quoteService;

        public QuoteController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        
        
        public async Task<ActionResult> GetQuote()
        {
            try
            {
                var quote = await _quoteService.GetRandomQuoteAsync();
                return Ok(quote);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new { message = "Quote service unavailable.", detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unexpected error.", detail = ex.Message });
            }
        }
    }
}
