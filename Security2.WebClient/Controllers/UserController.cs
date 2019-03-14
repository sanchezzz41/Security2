using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Security2.Dto.Models;
using Security2.WebClient.Services;

namespace Security2.WebClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userClient;

        public UserController(UserService userClient)
        {
            _userClient = userClient;
        }

        [HttpPost("login")]
        public async Task Authorize(UserLogin info)
        {
            await _userClient.Login(info);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, info.Email),
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
    }
}
