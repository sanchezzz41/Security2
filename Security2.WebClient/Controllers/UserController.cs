using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Logging;
using Security2.Dto.Models;
using Security2.WebClient.Services;

namespace Security2.WebClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userClient;
        private readonly ILogger<UserController> _logger;

        public UserController(UserService userClient,
            ILogger<UserController> logger)
        {
            _userClient = userClient;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task Authorize(UserLogin info)
        {
            var key = await _userClient.Login(info);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, info.Email),
                new Claim("GronsKey", key),
                new Claim(ClaimsIdentity.DefaultNameClaimType, info.Email)
            };
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        [HttpGet]
        public async Task<List<UserModel>> GetUsers()
        {
            var email = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            return await _userClient.GetUsers(email);
        }

        [Authorize]
        [HttpPost("SetKey")]
        public async Task SetKey(string key)
        {
            var email = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            await _userClient.SetKey(email, key);
        }
    }
}
