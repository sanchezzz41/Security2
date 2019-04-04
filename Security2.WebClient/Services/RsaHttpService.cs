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
using Security2.Rsa;
using Security2.WebClient.Models;
using Security2.WebClient.Utils;

namespace Security2.WebClient.Services
{
    /// <summary>
    /// Сервис для работы с клиентом
    /// </summary>
    public class RsaHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly RsaService _rsaService;
        private GronsfeldService _gronsfeldService;
        private readonly ILogger<NewsService> _logger;

        public RsaHttpService(HttpClient httpClient,
            IMemoryCache memoryCache,
            RsaService rsaService, ILogger<NewsService> logger,
            GronsfeldService gronsfeldService)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _rsaService = rsaService;
            _logger = logger;
            _gronsfeldService = gronsfeldService;
        }

        public async Task LoginWithRsa(ServiceInfo model)
        {
            _logger.LogInformation($"Пользователь {model.Email} ввел данные:{JsonConvert.SerializeObject(model)}");

            var res = await _httpClient.GetAsync($"rsa/RsaPublicKey");
            res.EnsureSuccessStatusCode();

            var serverPublicKey = await res.Content.ReadAsAsync<RsaPublicKey>();
            _logger.LogInformation($"Rsa:Публичный ключ с сервера: {await res.Content.ReadAsStringAsync()}");

            var encryptPassword = _rsaService.Encrypt(model.Password, serverPublicKey);
            _logger.LogInformation($"Зашифрованный пароль гронсфельдом:{encryptPassword}");

            var encryptKey = _rsaService.Encrypt(model.Key, serverPublicKey);
            _logger.LogInformation($"Зашифрованный Ключ пуб. ключом сервера:{encryptKey}");
            var gronsKey = model.Key.ToString();
            model.Key = encryptKey;
            model.Password = encryptPassword;

            var authResponse = await _httpClient.PostAsync($"rsa/LoginWithRsa", JsonContent.Convert(model));

            authResponse.EnsureSuccessStatusCode();

            _logger.LogInformation($"Ответ успешен");

            _memoryCache.Set(model.Email + ":serverPubKey", serverPublicKey);

            var cookie = authResponse.Headers.GetValues(CacheKeyCookieModel.GetCookieName);
            _memoryCache.Set(model.Email + "3", new CacheKeyCookieModel(gronsKey, cookie), TimeSpan.FromDays(1));

            _logger.LogInformation($"Куки установлены");
        }

        public async Task<Guid> CreateNews(string email, NewsInfo model)
        {
            var userModel = _memoryCache.Get<CacheKeyCookieModel>(email + "3");
            _logger.LogInformation($"Ключ пользователя {email} : {userModel.Key}");

            _httpClient.DefaultRequestHeaders.Add(CacheKeyCookieModel.SetCookie, userModel.Cookie);
            _logger.LogInformation($"Отправляемая модель на сервер перед шифрованием:{JsonConvert.SerializeObject(model)}");

            model.Title = _gronsfeldService.Encrypt(model.Title, userModel.Key);
            model.Content = _gronsfeldService.Encrypt(model.Content, userModel.Key);
            _logger.LogInformation($"Отправляемая модель на сервер после шифрования:{JsonConvert.SerializeObject(model)}");

            var response = await _httpClient.PostAsync($"rsa/News", JsonContent.Convert(model));

            var responseString = await response.Content.ReadAsStringAsync();
            var newsGuid = _gronsfeldService.Decrypt(responseString, userModel.Key);
            return Guid.Parse(newsGuid);
        }

        public async Task<List<NewsModel>> Get(string email)
        {
            var userModel = _memoryCache.Get<CacheKeyCookieModel>(email + "3");
            _logger.LogInformation($"Ключ пользователя {email} : {userModel.Key}");

            _httpClient.DefaultRequestHeaders.Add(CacheKeyCookieModel.SetCookie, userModel.Cookie);
            _logger.LogInformation($"Запрос на новости отпарвляется.");

            var response = await _httpClient.GetAsync($"rsa/News");

            var responseList = await response.Content.ReadAsAsync<List<NewsModel>>();
            _logger.LogInformation($"Пришедший список: {JsonConvert.SerializeObject(responseList)}");
            foreach (var item in responseList)
            {
                item.Content = _gronsfeldService.Decrypt(item.Content, userModel.Key);
                item.Title = _gronsfeldService.Decrypt(item.Title, userModel.Key);
            }
            _logger.LogInformation($"Пришедший список после дешифровки: {JsonConvert.SerializeObject(responseList)}");
            return responseList;
        }
    }
}
