using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.Application.Services;
using Test.Shared;

namespace Test.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Invalid request body");
            }

            if (model.Username.Length > 50 || model.Password.Length > 50)
            {
                return BadRequest("Username or password too long");
            }

            if (!IsValidUsernameFormat(model.Username) || !IsValidPasswordFormat(model.Password))
            {
                return BadRequest("Invalid username or password format");
            }

            if (_authService.ValidateUser(model.Username, model.Password))
            {
                string token = JwtService.GenerateJwtToken(model.Username);
                return Ok(new { token });
            }

            return Unauthorized("Invalid username or password");
        }

        private bool IsValidUsernameFormat(string username)
        {
            return true;
        }

        private bool IsValidPasswordFormat(string password)
        {
            return true;
        }

        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
