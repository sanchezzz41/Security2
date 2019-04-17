using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Security2.Dto.Models;
using Security2.Rsa;
using Security2.WebClient.Models;
using Security2.WebClient.Utils;

namespace Security2.WebClient.Controllers
{
    [ApiController, Route("[controller]")]
    public class TimePasswordController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly RsaService _rsaService;
        private readonly IMemoryCache _memoryCache;

        public TimePasswordController(RsaService rsaService, IMemoryCache memoryCache)
        {
            _rsaService = rsaService;
            _memoryCache = memoryCache;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000")
            };
        }

        [HttpPost("Register")]
        public async Task<Guid> Registration(UserRegisterWithTimePassword model)
        {
           var serverPublicKey = await GetPubKey();

           model.TimePasswords.FirstPass = _rsaService.Encrypt(model.TimePasswords.FirstPass, serverPublicKey);
           model.TimePasswords.SecondPass = _rsaService.Encrypt(model.TimePasswords.SecondPass, serverPublicKey);
           model.TimePasswords.ThreePass =_rsaService.Encrypt(model.TimePasswords.ThreePass, serverPublicKey);
           model.TimePasswords.FourPass = _rsaService.Encrypt(model.TimePasswords.FourPass, serverPublicKey);
           model.TimePasswords.FivePass = _rsaService.Encrypt(model.TimePasswords.FivePass, serverPublicKey);
           model.TimePasswords.SixPass =_rsaService.Encrypt(model.TimePasswords.SixPass, serverPublicKey);
           model.TimePasswords.SevenPass =_rsaService.Encrypt(model.TimePasswords.SevenPass, serverPublicKey);
            
           model.Email = _rsaService.Encrypt(model.Email, serverPublicKey);
           model.Name = _rsaService.Encrypt(model.Name, serverPublicKey);

           var responseGuid = await _httpClient.PostAsJsonAsync("TimePassword/Register", model);
           responseGuid.EnsureSuccessStatusCode();
           return await responseGuid.Content.ReadAsAsync<Guid>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns>Если true, то надо обновить пароли</returns>
        /// <exception cref="NullReferenceException"></exception>
        [HttpPost("Login")]
        public async Task<string> Login(UserLogin loginModel)
        {
            var pubKey = await GetPubKey();
            loginModel.Email = _rsaService.Encrypt(loginModel.Email, pubKey);
            loginModel.Password = _rsaService.Encrypt(loginModel.Password, pubKey);

            var responseBool = await _httpClient.PostAsJsonAsync("TimePassword/login", loginModel);
            responseBool.EnsureSuccessStatusCode();
            
            var cookie = responseBool.Headers.GetValues(CacheKeyCookieModel.GetCookieName);
            _memoryCache.Set(loginModel.Email+"4",cookie,  TimeSpan.FromDays(1));
            
  
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginModel.Email),
                new Claim(ClaimsIdentity.DefaultNameClaimType, loginModel.Email),
                new Claim(ClaimTypes.Email, loginModel.Email)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            if (await responseBool.Content.ReadAsAsync<bool>())
                return "Вам сегодня нужно поменять пароли.";
            return "Пароли ещё не нужно менять.";
        }
        
        [HttpPost("UpdatePasswords"), Authorize]
        public async Task UpdatePasswords(UserRegisterWithTimePassword model)
        {
            var serverPublicKey = await GetPubKey();

            model.TimePasswords.FirstPass = _rsaService.Encrypt(model.TimePasswords.FirstPass, serverPublicKey);
            model.TimePasswords.SecondPass = _rsaService.Encrypt(model.TimePasswords.SecondPass, serverPublicKey);
            model.TimePasswords.ThreePass =_rsaService.Encrypt(model.TimePasswords.ThreePass, serverPublicKey);
            model.TimePasswords.FourPass = _rsaService.Encrypt(model.TimePasswords.FourPass, serverPublicKey);
            model.TimePasswords.FivePass = _rsaService.Encrypt(model.TimePasswords.FivePass, serverPublicKey);
            model.TimePasswords.SixPass =_rsaService.Encrypt(model.TimePasswords.SixPass, serverPublicKey);
            model.TimePasswords.SevenPass =_rsaService.Encrypt(model.TimePasswords.SevenPass, serverPublicKey);
            
            model.Email = _rsaService.Encrypt(model.Email, serverPublicKey);
            model.Name = _rsaService.Encrypt(model.Name, serverPublicKey);

            var cookie = _memoryCache.Get<IEnumerable<string>>(HttpContext.GetEmail() + "4");

            _httpClient.DefaultRequestHeaders.Add(CacheKeyCookieModel.SetCookie,cookie);            
            
            var responseGuid = await _httpClient.PostAsJsonAsync("TimePassword/UpdatePasswords", model);
            responseGuid.EnsureSuccessStatusCode();
        }

        private async Task<RsaPublicKey> GetPubKey()
        {
            var responsePubKey = await _httpClient.GetAsync("rsa/RsaPublicKey");
            responsePubKey.EnsureSuccessStatusCode();

            return await responsePubKey.Content.ReadAsAsync<RsaPublicKey>();
        }
    }
}