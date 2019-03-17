using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Security2.Dto.Models;
using Security2.WebClient.Services;
using Security2.WebClient.Utils;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Security2.WebClient.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class RsaClientController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly RsaHttpService _rsaHttpService;
        public RsaClientController(IMemoryCache memoryCache, RsaHttpService rsaHttpService)
        {
            _memoryCache = memoryCache;
            _rsaHttpService = rsaHttpService;
        }

        [HttpPost("SetKeyRsa")]
        public async Task SetKey()
        {
            var email = HttpContext.GetEmail();
            await _rsaHttpService.SetKey(email);
        }

        [HttpPost("News")]
        public async Task<Guid> CreateNews(NewsInfo model)
        {
            var email = HttpContext.GetEmail();
            return await _rsaHttpService.CreateNews(email, model);
        }

        [HttpGet("News")]
        public async Task<List<NewsModel>> Get()
        {
            var email = HttpContext.GetEmail();
            return await _rsaHttpService.Get(email);
        }
    }
}
