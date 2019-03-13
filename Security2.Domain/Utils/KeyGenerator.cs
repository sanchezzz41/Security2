using System;
using Security2.Gronsfer;

namespace Security2.Domain.Utils
{
    public class KeyGenerator
    {
        public static string ClaimType = "GronsfeldKey";
        private readonly GronsfeldOptions _options;

        public KeyGenerator(GronsfeldOptions options)
        {
            _options = options;
        }

        public string GenerateKey()
        {
            var resultKey = "";
            var randomizer = new Random();
            for (int i = 0; i < _options.KeyLenght; i++)
            {
                resultKey += randomizer.Next(1, 9);
            }

            return resultKey;
        }
    }
}
