using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Security2.WebClient.Models
{
    public class KeyModel
    {
        public static string GetCookieName = "Set-Cookie";
        public static string SetCookie = "Cookie";

        public string Key { get; set; }

        public IEnumerable<string> Cookie { get; set; }

        /// <inheritdoc />
        public KeyModel(string key, IEnumerable<string> cookie)
        {
            Key = key;
            Cookie = cookie;
        }

    }
}
