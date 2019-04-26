using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Security2.Domain.Services;
using Security2.Dto.Models;
using Security2.Web.Utils.Binders;
using Security2.Web.Utils.ResultFilter;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Security2.Web.Controllers
{
    /// <summary>
    /// Контроллер для работы с новостоями
    /// </summary>
    [ApiController, Route("[controller]")]
    public class NewsController : Controller
    {
        private readonly NewsService _newsService;

        public NewsController(NewsService newsService)
        {
            _newsService = newsService;
        }

        /// <summary>
        /// Созд новости
        /// </summary>
        /// <param name="data"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [JsonEncryptResultFilter]
        [HttpPost]
        public async Task<Guid> Create(string data, [ModelBinder(typeof(CustomJsonModelBinder))] NewsInfo model)
        {
            return await _newsService.Create(model);
        }

        /// <summary>
        /// Получение новости
        /// </summary>
        /// <returns></returns>
        [JsonEncryptResultFilter]
        [HttpGet]
        public async Task<List<NewsModel>> Get()
        {
            var headers = HttpContext.Request.Headers;
            return await _newsService.Get();
        }

        [HttpGet("Single"), JsonEncryptResultFilter]
        public async Task<NewsModel> GetById(string id, [ModelBinder(typeof(PathModelBinder))] Guid newsGuid)
        {
            var result = await _newsService.Get();
            return result.Single(x => x.Id == newsGuid);
        }
    }
}
