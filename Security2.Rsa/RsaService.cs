using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Security2.Rsa
{
    /// <summary>
    /// Сервис для работы с RSA
    /// </summary>
    public class RsaService
    {
        private readonly ILogger<RsaService> _logger;

        public RsaService(ILogger<RsaService> logger)
        {
            _logger = logger;
        }

        public RsaService()
        {
        }

        /// <summary>
        /// Шифрование данных
        /// </summary>
        /// <param name="model"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public string Encrypt(object model, RsaPublicKey publicKey)
        {
            var text = JsonConvert.SerializeObject(model);
            var enc = new RSACryptoServiceProvider();
            var rsaParametr = publicKey.GetRsaParameters();
            enc.ImportParameters(rsaParametr);
            return Convert.ToBase64String(enc.Encrypt(Encoding.ASCII.GetBytes(text), false));
        }

        /// <summary>
        /// Дешифрование
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privateKey"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Decrypt<T>(string data, RSAParameters privateKey)
        {
            var enc = new RSACryptoServiceProvider();
            enc.ImportParameters(privateKey);
            var resultData = Encoding.ASCII.GetString(enc.Decrypt(Convert.FromBase64String(data), false));
            return JsonConvert.DeserializeObject<T>(resultData);
        }

        /// <summary>
        /// Дешифрование
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privateKey"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string Decrypt(string data, RSAParameters privateKey)
        {
            var enc = new RSACryptoServiceProvider();
            enc.ImportParameters(privateKey);
            var resultData = Encoding.ASCII.GetString(enc.Decrypt(Convert.FromBase64String(data), false));
            return resultData;
        }

        public static RsaKeys GetKeyPair(int length = 2048)
        {
            var encProvider = new RSACryptoServiceProvider(length);
            var publicKey = encProvider.ExportParameters(false);
            var privateKey = encProvider.ExportParameters(true);
            return new RsaKeys(publicKey, privateKey);
        }
    }
}