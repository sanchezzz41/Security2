using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Refit;
using Security2.Domain.Models;

namespace Security2.WebClient.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Login(UserLogin model)
        {
           var result = await _httpClient.PostAsync("user/login", JsonContent.Convert(model));
        }
    }

    public static class JsonContent
    {
        public static StringContent Convert(object obj)
        {
            var result = JsonConvert.SerializeObject(obj);
            return new StringContent(result, Encoding.UTF8, "application/json");
        }
    }
}
