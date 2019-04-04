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

        public string Encrypt(object model, RsaPublicKey publicKey)
        {
            var text = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"Rsa: \nВходная модель: {text}");
            var enc = new RSACryptoServiceProvider();
            var rsaParametr = publicKey.GetRsaParameters();
            enc.ImportParameters(rsaParametr);
            return Convert.ToBase64String(enc.Encrypt(Encoding.ASCII.GetBytes(text), false));
        }

        public T Decrypt<T>(string data, RSAParameters privateKey)
        {
            var enc = new RSACryptoServiceProvider();
            enc.ImportParameters(privateKey);
            var resultData = Encoding.ASCII.GetString(enc.Decrypt(Convert.FromBase64String(data), false));
            _logger.LogInformation($"Rsa: \nРезультат: {resultData}");
            return JsonConvert.DeserializeObject<T>(resultData);
        }

        public string Decrypt(string data, RSAParameters privateKey)
        {
            var enc = new RSACryptoServiceProvider();
            enc.ImportParameters(privateKey);
            var resultData = Encoding.ASCII.GetString(enc.Decrypt(Convert.FromBase64String(data), false));
            _logger.LogInformation($"Rsa: \nРезультат: {resultData}");
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