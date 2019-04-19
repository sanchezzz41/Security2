using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClientWPF
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Отправляет файл в формате json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string uri,  T model) where T : class
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            return await client.PostAsync(uri, content);
        }

        public static async Task<T> ReadAsAsync<T>(this HttpContent response)
        {
            var content = await response.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}