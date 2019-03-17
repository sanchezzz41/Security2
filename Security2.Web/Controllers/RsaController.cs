using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Security2.Domain.Services;
using Security2.Dto.Models;
using Security2.Rsa;
using Security2.Web.Utils;
using Security2.Web.Utils.Binders;
using Security2.Web.Utils.ResultFilter;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Security2.Web.Controllers
{
    [ApiController, Authorize, Route("[controller]")]
    public class RsaController : Controller
    {
        private readonly RsaServerKeys _rsaServerKeys;
        private readonly IMemoryCache _memoryCache;
        private readonly NewsService _newsService;

        public RsaController(RsaServerKeys rsaServerKeys, IMemoryCache memoryCache, NewsService newsService)
        {
            _rsaServerKeys = rsaServerKeys;
            _memoryCache = memoryCache;
            _newsService = newsService;
        }

        /// <summary>
        /// Устанавливает на сервере публичный ключ
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns>Возвращает публичный ключ сервера</returns>
        [HttpPost("SetRsaPublicKey")]
        public async Task<RsaPublicKey> SetKey(RsaPublicKey publicKey)
        {
            var email = HttpContext.GetEmail();
            _memoryCache.Set(RsaExtensions.GetUserId(email), publicKey, TimeSpan.FromDays(1));
            return new RsaPublicKey(_rsaServerKeys.PublicKey);
        }


        /// <summary>
        /// Отправлять вместе с data в query
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        [HttpPost("News")]
        [RsaJsonEncryptFilter]
        public async Task<Guid> CreateTest([ModelBinder(typeof(RsaJsonModelBinder))] NewsInfo test, string data)
        {
          return  await _newsService.Create(test);
        }

        [HttpGet("News"), RsaJsonEncryptFilter]
        public async Task<List<NewsModel>> GetNews()
        {
            return await _newsService.Get();
        }
    }
}
