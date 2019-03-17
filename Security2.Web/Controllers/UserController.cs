using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Security2.Database.Entities;
using Security2.Domain.Services;
using Security2.Domain.Utils;
using Security2.Dto.Models;
using Security2.Web.Utils;
using Security2.Web.Utils.ResultFilter;

namespace Security2.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly UserAccount _userAccount;
        private readonly KeyGenerator _keyGenerator;
        private readonly ILogger<UserController> _logger;
        private readonly IMemoryCache _memoryCache;

        public UserController(UserAccount userAccount,
            KeyGenerator keyGenerator, ILogger<UserController> logger,
            IMemoryCache memoryCache)
        {
            _userAccount = userAccount;
            _keyGenerator = keyGenerator;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpPost("Registration")]
        public async Task<Guid> Registration(UserInfo model)
        {
            return await _userAccount.Registration(model);
        }


        [HttpGet]
        [Authorize]
        [JsonEncryptResultFilter]
        public async Task<List<User>> GetUsers()
        {
            return await _userAccount.GetUsers();
        }

        [HttpGet("Original")]
        [Authorize]
        [JsonEncryptResultFilter]
        public async Task<List<UserModel>> GetOriginalUsers()
        {
            return await _userAccount.GetOriginalUsers();
        }

        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Возвращает ключ</returns>
        [HttpPost("Login")]
        public async Task Login(UserLogin model)
        {
            var user = _userAccount.Login(model);
            if (user == null)
                throw new NullReferenceException("Введенны неверные данные.");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            _logger.LogInformation($"Пользователь: {user.Name} зашёл на сайт.");
        }

        [HttpPost("Key"), Authorize]
        public void SetKey(string key)
        {
            var email = HttpContext.GetEmail();
            _logger.LogInformation($"Пользователь:{email} установил себе ключ:{key}");
            _memoryCache.Set(email, key, TimeSpan.FromDays(1));
        }

        [HttpGet("Logout")]
        [Authorize]
        public async Task LogOut()
        {
            _logger.LogInformation($"Пользователь вышел с сайта.");
            await HttpContext.SignOutAsync();
        }
    }
}