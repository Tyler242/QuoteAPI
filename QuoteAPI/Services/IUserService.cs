using QuoteAPI.Models;

namespace QuoteAPI.Services
{
    public interface IUserService
    {
        Task<UserDTO?> ValidateUser(UserLogin userLogin);
        Task<bool> AddUser(UserModel userModel);
    }
}
