using QuoteAPI.Models;

namespace QuoteAPI.Services
{
    public interface IUserService
    {
        Task<UserDTO>? GetUser(UserLogin userLogin);
        Task<bool> AddUser(UserModel userModel);
    }
}
