using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Security2.Domain.Services;
using Security2.Dto.Models;
using Security2.Gronsfer;
using Security2.Rsa;
using Security2.Web.Utils;
using Security2.Web.Utils.Binders;
using Security2.Web.Utils.ResultFilter;


namespace Security2.Web.Controllers
{
    [ApiController, Route("[controller]")]
    public class RsaController : Controller
    {
        private readonly RsaServerKeys _rsaServerKeys;
        private readonly GronsfeldService _gronsfeldService;
        private readonly RsaService _rsaService;
        private readonly IMemoryCache _memoryCache;
        private readonly NewsService _newsService;
        private readonly UserAccount _userAccount;
        private readonly ILogger<RsaController> _logger;

        public RsaController(RsaServerKeys rsaServerKeys, 
            IMemoryCache memoryCache, 
            NewsService newsService, 
            GronsfeldService gronsfeldService,
            RsaService rsaService,
            UserAccount userAccount, ILogger<RsaController> logger)
        {
            _rsaServerKeys = rsaServerKeys;
            _memoryCache = memoryCache;
            _newsService = newsService;
            _gronsfeldService = gronsfeldService;
            _rsaService = rsaService;
            _userAccount = userAccount;
            _logger = logger;
        }

        /// <summary>
        /// Устанавливает на сервере сим ключ
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns>Возвращает публичный ключ сервера</returns>
        [HttpPost("LoginWithRsa"), AllowAnonymous]
        public async Task SetKey(ServiceInfo serviceInfo)
        {
            _logger.LogInformation($"Входная модель:{JsonConvert.SerializeObject(serviceInfo)}");

            var email = serviceInfo.Email;
            var decryptKey = _rsaService.Decrypt(serviceInfo.Key, _rsaServerKeys.PrivateKey).Replace("\"", "");
            _logger.LogInformation($"Дешифрованный приватным ключом, Ключ гронсфельда:{decryptKey}");

            var password = _rsaService.Decrypt<string>(serviceInfo.Password, _rsaServerKeys.PrivateKey);
            _logger.LogInformation($"Дешифрованный пароль:{password}");
            var user = _userAccount.Login(new UserLogin
            {
                Email = email,
                Password = password
            });
            if(user==null)
                throw new NullReferenceException("Такого пользователя не существует.");
            _memoryCache.Set(email + "3", new GronsKeyModel {Key = decryptKey});
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, serviceInfo.Email),
                new Claim(ClaimsIdentity.DefaultNameClaimType, serviceInfo.Email),
                new Claim(ClaimTypes.Email, serviceInfo.Email)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("RsaPublicKey")]
        public async Task<RsaPublicKey> GetPublicKey()
        {
            return new RsaPublicKey(_rsaServerKeys.PublicKey);
        }

        [HttpPost("News")]
        public async Task<string> CreateNews(NewsInfo model)
        {
            var key = _memoryCache.Get<GronsKeyModel>(HttpContext.GetEmail() + "3").Key;
            _logger.LogInformation($"Ключ пользователя {HttpContext.GetEmail()} : {key}");
            _logger.LogInformation("Входная модель новости: " + JsonConvert.SerializeObject(model));
            model.Title = _gronsfeldService.Decrypt(model.Title, key);
            model.Content = _gronsfeldService.Decrypt(model.Content, key);
            _logger.LogInformation("Входная модель новости после дешифровки: " + JsonConvert.SerializeObject(model));

            var result = await _newsService.Create(model);
            return _gronsfeldService.Encrypt(result.ToString(), key);
        }

        [HttpGet("News")]
        public async Task<List<NewsModel>> Get()
        {
            var key = _memoryCache.Get<GronsKeyModel>(HttpContext.GetEmail() + "3").Key;
            _logger.LogInformation($"Ключ пользователя {HttpContext.GetEmail()} : {key}");
            var result = await _newsService.Get();

            foreach (var newsModel in result)
            {
                newsModel.Content = _gronsfeldService.Encrypt(newsModel.Content, key);
                newsModel.Title = _gronsfeldService.Encrypt(newsModel.Title, key);
            }

            _logger.LogInformation($"Отправляемая модель: {JsonConvert.SerializeObject(result)}");

            return result;
        }
    }
}
