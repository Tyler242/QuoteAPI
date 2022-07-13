using QuoteAPI.Models;

namespace QuoteAPI.Services
{
    public interface IUserService
    {
        Task<UserDTO>? GetUser(UserModel userModel);
        Task<bool> AddUser(UserModel userModel);
    }
}
