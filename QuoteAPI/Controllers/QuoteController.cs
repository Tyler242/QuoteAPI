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
        public List<Quote> GetAllQuotes([FromQuery] string? word = null, string? source = null, string? tag = null) =>
            _quoteService.GetQuotes(word, source, tag);

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Quote>> GetQuoteById(string id)
        {
            Quote quote = await _quoteService.GetQuoteByIdAsync(id);

            if (quote == null)
                return NotFound();

            return Ok(quote);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuote(QuoteCreationModel quoteModel) =>
            Ok(await _quoteService.CreateQuoteAsync(quoteModel));

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateQuote(string id, QuoteUpdateModel updateModel)
        {
            Quote? quote = await _quoteService.UpdateQuoteAsync(id, updateModel);

            if (quote == null)
                return NotFound();

            return Ok(quote);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<ActionResult> DeleteQuote(string id)
        {
            Quote? quote = await _quoteService.RemoveQuoteAsync(id);

            if (quote == null)
                return NotFound();

            return Ok(quote);
        }
    }
}
