using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuoteAPI.Models;
using QuoteAPI.Services;

namespace QuoteAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private string? _token = null;

        public UserController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            if (string.IsNullOrEmpty(userLogin.UserName) || string.IsNullOrEmpty(userLogin.Password))
                return BadRequest();

            UserDTO? validUser = await GetUser(userLogin);

            if (validUser != null)
            {
                _token = _tokenService.BuildToken(validUser);
                if (_token != null)
                {
                    return Ok($"Authenticated with {_token}");
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [Route("signup")]
        [HttpPost]
        public async Task<IActionResult> SignUp(UserModel userModel)
        {
            if (string.IsNullOrEmpty(userModel.UserName) || string.IsNullOrEmpty(userModel.Password))
                return BadRequest();

            var result = await _userService.AddUser(userModel);

            if (result)
                return Ok();
            else
                return BadRequest();
        }

        private async Task<UserDTO?> GetUser(UserLogin userLogin)
        {
            UserDTO? user = await _userService.GetUser(userLogin);

            if (user == null)
                return null;

            return user;
        }
    }
}
