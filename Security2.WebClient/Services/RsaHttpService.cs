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
    public class RsaHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly RsaService _rsaService;
        private readonly ILogger<NewsService> _logger;

        public RsaHttpService(HttpClient httpClient,
            IMemoryCache memoryCache,
            RsaService rsaService, ILogger<NewsService> logger)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _rsaService = rsaService;
            _logger = logger;
        }

        public async Task SetKey(string email)
        {
            var key = RsaService.GetKeyPair(4096);
            var userKeys = new ClientRsaKeys();
            userKeys.RsaPublicKeyUser = new RsaPublicKey(key.PublicKey);
            userKeys.RsaPrivateKeyUser = key.PrivateKey;
            _logger.LogInformation($"Rsa: Сгенерированные ключи:\n Pub:{userKeys.RsaPublicKeyUser.Exponent} / {userKeys.RsaPublicKeyUser.Modulus}");

            var cookie = _memoryCache.Get<CacheKeyCookieModel>(email).Cookie;

            _httpClient.DefaultRequestHeaders.Add(CacheKeyCookieModel.SetCookie, cookie);

            _logger.LogInformation($"Устанавливаем ключ на сервере");

            var res = await _httpClient.PostAsync($"rsa/SetRsaPublicKey",
                JsonContent.Convert(userKeys.RsaPublicKeyUser));

            var serverPublicKey = await res.Content.ReadAsAsync<RsaPublicKey>();
            _logger.LogInformation($"Rsa: Ответ с сервера: {await res.Content.ReadAsStringAsync()}");
            userKeys.RsaPublicKeyServer = serverPublicKey;
            _memoryCache.Set(RsaExtensions.GetUserId(email), userKeys);
        }

        public async Task<Guid> CreateNews(string email, NewsInfo model)
        {
            var keys = _memoryCache.Get<ClientRsaKeys>(RsaExtensions.GetUserId(email));

            var cookie = _memoryCache.Get<CacheKeyCookieModel>(email).Cookie;

            _httpClient.DefaultRequestHeaders.Add(CacheKeyCookieModel.SetCookie, cookie);

            var encryptData = _rsaService.Encrypt(model, keys.RsaPublicKeyServer);
            var res = await _httpClient.PostAsync($"rsa/News?data={encryptData}",null);

            var resultString = await res.Content.ReadAsStringAsync();
            var encryptModel = _rsaService.Decrypt(resultString, keys.RsaPrivateKeyUser);
            return JsonConvert.DeserializeObject<Guid>(encryptModel);
        }

        public async Task<List<NewsModel>> Get(string email)
        {
            var keys = _memoryCache.Get<ClientRsaKeys>(RsaExtensions.GetUserId(email));

            var cookie = _memoryCache.Get<CacheKeyCookieModel>(email).Cookie;

            _httpClient.DefaultRequestHeaders.Add(CacheKeyCookieModel.SetCookie, cookie);

            var res = await _httpClient.GetAsync($"rsa/News");
            res.EnsureSuccessStatusCode();
            var resultString = await res.Content.ReadAsStringAsync();
            return _rsaService.Decrypt<List<NewsModel>>(resultString, keys.RsaPrivateKeyUser);
        }
    }
}
