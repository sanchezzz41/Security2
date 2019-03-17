using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Security2.Rsa;

namespace Security2.Web.Utils.Binders
{
    public class RsaJsonModelBinder : IModelBinder
    {
        private readonly RsaService _rsaService;
        private readonly ILogger<RsaJsonModelBinder> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly RsaServerKeys _rsaServerKeys;

        public RsaJsonModelBinder(ILogger<RsaJsonModelBinder> logger,
            RsaService rsa, IMemoryCache memoryCache,
            RsaServerKeys rsaServerKeys)
        {
            _logger = logger;
            _rsaService = rsa;
            _memoryCache = memoryCache;
            _rsaServerKeys = rsaServerKeys;
        }

        /// <inheritdoc />
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var request = bindingContext.HttpContext.Request;
            var type = bindingContext.ModelType;
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var query = request.Query["data"].ToString().Replace(" ", "+");
            _logger.LogInformation($"RSA");
            _logger.LogInformation($"Данные с запроса:{query}");
            //var email = bindingContext.HttpContext.User.FindFirst(ClaimTypes.Email).Value ;
            //var key = _memoryCache.Get<RsaKeys>(RsaExtensions.GetUserId(email));
            var decryptModel = _rsaService.Decrypt(query, _rsaServerKeys.PrivateKey);
            var resultData = JsonConvert.DeserializeObject(decryptModel, type);
            _logger.LogInformation(
                $"Входные данные:{query}\nРезультат дешифровки:{JsonConvert.SerializeObject(resultData)}");
            bindingContext.Result = ModelBindingResult.Success(resultData);
        }
    }
}
