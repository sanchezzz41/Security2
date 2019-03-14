using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Security2.WebClient.Utils
{
    public static class JsonContent
    {
        public static StringContent Convert(object obj)
        {
            var result = JsonConvert.SerializeObject(obj);
            return new StringContent(result, Encoding.UTF8, "application/json");
        }
    }
}