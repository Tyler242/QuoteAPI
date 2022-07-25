using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuoteAPI.Models;
using QuoteAPI.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace QuoteAPI.Controllers
{
    [Authorize(Roles = "Admin,User")]
    [ApiController]
    [Route("api/quotes")]
    public class QuoteController : ControllerBase
    {
        private readonly QuoteService _quoteService;
        private readonly ITokenService _tokenService;

        public QuoteController(QuoteService quoteService, ITokenService tokenService)
        {
            _quoteService = quoteService;
            _tokenService = tokenService;
        }

        [HttpGet]
        public ActionResult<List<Quote>> GetAllQuotes([FromQuery] string? word = null, string? source = null, string? tag = null)
        {
            string userId = GetUserId();

            if (userId.Equals(string.Empty))
                return BadRequest("Failed to get UserId");

            return Ok(_quoteService.GetQuotes(userId, word, source, tag));
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Quote>> GetQuoteById([FromRoute] string id)
        {
            string userId = GetUserId();

            if (userId.Equals(string.Empty))
                return BadRequest("Failed to get UserId");

            Quote quote = await _quoteService.GetQuoteByIdAsync(userId, id);

            if (quote == null)
                return NotFound();

            return Ok(quote);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuote([FromBody] QuoteCreationModel quoteModel)
        {
            string userId = GetUserId();

            if (userId.Equals(string.Empty))
                return BadRequest("Failed to get UserId");
            else 
                return Ok(await _quoteService.CreateQuoteAsync(quoteModel, userId));
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateQuote([FromRoute] string id, [FromBody] QuoteUpdateModel updateModel)
        {
            string userId = GetUserId();

            if (userId.Equals(string.Empty))
                return BadRequest("Failed to get UserId");

            Quote? quote = await _quoteService.UpdateQuoteAsync(userId, id, updateModel);

            if (quote == null)
                return NotFound();

            return Ok(quote);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<ActionResult> DeleteQuote([FromRoute] string id)
        {
            string userId = GetUserId();

            if (userId.Equals(string.Empty))
                return BadRequest("Failed to get UserId");

            Quote? quote = await _quoteService.RemoveQuoteAsync(userId, id);

            if (quote == null)
                return NotFound();

            return Ok(quote);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/all")]
        public ActionResult<List<Quote>> AdminGetAllQuotes([FromQuery] string? word = null, string? source = null, string? tag = null, string? userId = null)
        {
            return Ok(_quoteService.AdminGetAllQuotes(word, source, tag, userId));
        }


        private string GetUserId()
        {
            if (User.Identity != null && User.Identity.Name != null)
                return User.Identity.Name;
            else
                return string.Empty;
        }
    }
}
