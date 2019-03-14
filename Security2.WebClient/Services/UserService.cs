using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Refit;
using Security2.Dto.Models;
using Security2.Gronsfer;
using Security2.WebClient.Models;
using Security2.WebClient.Utils;

namespace Security2.WebClient.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly GronsfeldService _gronsfeldService;

        public UserService(HttpClient httpClient, IMemoryCache memoryCache,
            GronsfeldService gronsfeldService)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _gronsfeldService = gronsfeldService;
        }

        public async Task Login(UserLogin model)
        {
            var result = await _httpClient.PostAsync("user/login", JsonContent.Convert(model));
            result.EnsureSuccessStatusCode();
            if (!result.Headers.Contains(KeyModel.GetCookieName))
                throw new AuthenticationException("Такого логина/пароля не существует");
            var cookie = result.Headers.GetValues(KeyModel.GetCookieName);
            var key = await result.Content.ReadAsStringAsync();
            _memoryCache.Set(model.Email, new KeyModel(key, cookie), TimeSpan.FromDays(1));
        }

        public async Task<List<UserModel>> GetUsers(string userEmail)
        {
            var storageModel = _memoryCache.Get<KeyModel>(userEmail);
            _httpClient.DefaultRequestHeaders.Add(KeyModel.GetCookieName, storageModel.Cookie);
            var response = await _httpClient.GetAsync("user/original");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var res = _gronsfeldService.Decrypt(content, storageModel.Key);
            return JsonConvert.DeserializeObject<List<UserModel>>(res);
        }
    }
}
