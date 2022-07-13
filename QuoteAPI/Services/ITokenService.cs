using QuoteAPI.Models;

namespace QuoteAPI.Services
{
    public interface ITokenService
    {
        string BuildToken(UserDTO user);
        bool IsTokenValid(string token);
    }
}
