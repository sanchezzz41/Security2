using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Security2.Dto.Models;
using Security2.Gronsfer;
using Security2.WebClient.Models;

namespace Security2.WebClient.Services
{
    public class NewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly GronsfeldService _gronsfeldService;
        private readonly ILogger<NewsService> _logger;

        public NewsService(HttpClient httpClient,
            IMemoryCache memoryCache,
            GronsfeldService gronsfeldService, ILogger<NewsService> logger)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _gronsfeldService = gronsfeldService;
            _logger = logger;
        }

        public async Task<Guid> CreateNews(NewsInfo model, string email)
        {
            var keyModel = _memoryCache.Get<KeyModel>(email);
            _httpClient.DefaultRequestHeaders.Add(KeyModel.SetCookie, keyModel.Cookie);
            var json = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"Отправляемая модель:{json}");
            var encryptStr = _gronsfeldService.Encrypt(json, keyModel.Key);
            var res = await _httpClient.PostAsync($"news?data={encryptStr}", null);
            var content = await res.Content.ReadAsStringAsync();
            _logger.LogInformation($"Ответ с сервера:{content}");
            var afterDecrypt = _gronsfeldService.Decrypt(content, keyModel.Key);
            _logger.LogInformation($"После дешефровки:{content}");
            return JsonConvert.DeserializeObject<Guid>(afterDecrypt);
        }

        public async Task<List<NewsModel>> GetNews(string email)
        {
            var keyModel = _memoryCache.Get<KeyModel>(email);
            _httpClient.DefaultRequestHeaders.Add(KeyModel.SetCookie, keyModel.Cookie);
            var res = await _httpClient.GetAsync($"news");
            var content = await res.Content.ReadAsStringAsync();
            _logger.LogInformation($"Ответ с сервера:{content}");
            var afterDecrypt = _gronsfeldService.Decrypt(content, keyModel.Key);
            _logger.LogInformation($"После дешефровки:{afterDecrypt}");
            return JsonConvert.DeserializeObject<List<NewsModel>>(afterDecrypt);
        }
    }
}
