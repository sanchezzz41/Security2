using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Security2.Dto.Models;
using Security2.WebClient.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Security2.WebClient.Controllers
{
    [ApiController]
    [Authorize, Route("[controller]")]
    public class NewsController : Controller
    {
        private readonly NewsService _newsService;

        public NewsController(NewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpPost]
        public async Task<Guid> CreateNews(NewsInfo model)
        {
            var email = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            return await _newsService.CreateNews(model, email);
        }

        [HttpGet]
        public async Task<List<NewsModel>> GetNews()
        {
            var email = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            return await _newsService.GetNews(email);
        }

    }
}
