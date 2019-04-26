using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Security2.Domain.Utils;
using Security2.Gronsfer;

namespace Security2.Web.Utils.Binders
{
    /// <summary>
    /// Класс для дешифрования модели JSON
    /// </summary>
    public class CustomJsonModelBinder : IModelBinder
    {
        private readonly GronsfeldService _gronsfeldService;
        private readonly ILogger<CustomJsonModelBinder> _logger;
        private readonly IMemoryCache _memoryCache;

        public CustomJsonModelBinder(GronsfeldService gronsfeldService,
            ILogger<CustomJsonModelBinder> logger, IMemoryCache memoryCache)
        {
            _gronsfeldService = gronsfeldService;
            _logger = logger;
            _memoryCache = memoryCache;
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
            _logger.LogInformation($"Данные пришедшие с клиента:{query}");
            var key = _memoryCache.Get<string>(bindingContext.HttpContext.GetEmail());
            //var key = bindingContext.HttpContext.User.FindFirst(KeyGenerator.ClaimType).Value;
            var decryptModel = _gronsfeldService.Decrypt(query, key);
            var resultData = JsonConvert.DeserializeObject(decryptModel, type);
            _logger.LogInformation(
                $"Входные данные:{query}\nКлюч пользователя:{key}\nРезультат дешифровки:{JsonConvert.SerializeObject(resultData)}");
            bindingContext.Result = ModelBindingResult.Success(resultData);
        }
    }
}
