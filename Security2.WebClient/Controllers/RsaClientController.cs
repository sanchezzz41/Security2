using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Security2.Dto.Models;
using Security2.WebClient.Services;
using Security2.WebClient.Utils;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Security2.WebClient.Controllers
{
    [ApiController, Route("[controller]")]
    public class RsaClientController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly RsaHttpService _rsaHttpService;
        public RsaClientController(IMemoryCache memoryCache, RsaHttpService rsaHttpService)
        {
            _memoryCache = memoryCache;
            _rsaHttpService = rsaHttpService;
        }

        [HttpPost("LoginWithRsa")]
        public async Task SetKey(ServiceInfo model)
        {
            await _rsaHttpService.LoginWithRsa(model);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimsIdentity.DefaultNameClaimType, model.Email)
            };
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        [HttpPost("News")]
        public async Task<Guid> CreateNews(NewsInfo model)
        {
            return await _rsaHttpService.CreateNews(HttpContext.GetEmail(), model);
        }

        [HttpGet("News")]
        public async Task<List<NewsModel>> Get()
        {
            return await _rsaHttpService.Get(HttpContext.GetEmail());
        }
    }
}
