using Microsoft.AspNetCore.Mvc;
using QuoteAPI.Models;
using QuoteAPI.Services;

namespace QuoteAPI.Controllers
{
    [ApiController]
    [Route("api/quotes")]
    public class QuoteController : ControllerBase
    {
        private readonly QuoteService _quoteService;

        public QuoteController(QuoteService quoteService) =>
            _quoteService = quoteService;

        [HttpGet]
        public async Task<List<Quote>> GetAllQuotes() =>
            await _quoteService.GetQuotesAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Quote>> GetQuoteById(string id)
        {
            Quote quote = await _quoteService.GetQuoteByIdAsync(id);

            if (quote == null)
            {
                return NotFound();
            }

            return quote;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuote(Quote quoteModel)
        {
            await _quoteService.CreateQuoteAsync(quoteModel);

            return CreatedAtAction(nameof(GetQuoteById), new { id = quoteModel.Id }, quoteModel);
        }
    }
}
