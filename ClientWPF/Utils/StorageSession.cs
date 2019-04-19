using System.Collections.Generic;
using System.Windows;

namespace ClientWPF
{
    /// <summary>
    /// Класс для хранения данных о сессии
    /// </summary>
    public class StorageSession
    {
        public IEnumerable<string> Cookie { get; set; }

        public string Key { get; set; }

        public static StorageSession Context { get; set; }

        public static string GetCookieName = "Set-Cookie";
        public static string SetCookieName = "Cookie";

        private StorageSession()
        {
        }

        public static StorageSession Create()
        {
            Context = Context ?? new StorageSession();
            return Context;
        }

        public static StorageSession SetCookie(IEnumerable<string> cookie)
        {
            if (Context == null)
            {
                var res = new StorageSession();
                res.Cookie = cookie;
                Context = res;
            }
            else
                Context.Cookie = cookie;
            return Context;
        }

        public static StorageSession SetKey(string key)
        {
            if (Context == null)
            {
                var res = new StorageSession();
                res.Key = key;
                Context = res;
            }
            else
                Context.Key = key;
            return Context;
        }
    }
}
