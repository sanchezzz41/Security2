using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Security2.Domain.Utils;
using Security2.Gronsfer;

namespace Security2.Web.Utils.ResultFilter
{
    /// <summary>
    /// Класс для возвращения зашифрованной модели
    /// </summary>
    public class JsonEncryptResultFilter : Attribute, IResultFilter
    {
        private ILogger _logger;
        private GronsfeldService _gronsfeldService;
        private IMemoryCache _memoryCache;

        public void OnResultExecuting(ResultExecutingContext context)
        {
            var serviceProviders = context.HttpContext.RequestServices.CreateScope().ServiceProvider;
            _logger = serviceProviders.GetService<ILoggerProvider>()
                .CreateLogger("JsonEncryptResultFilter");
            _gronsfeldService = serviceProviders.GetService<GronsfeldService>();
            _memoryCache = serviceProviders.GetService<IMemoryCache>();
            var objResult = (ObjectResult) context.Result;
            var key = _memoryCache.Get<string>(context.HttpContext.GetEmail());
            var encryptData = JsonConvert.SerializeObject(objResult.Value);
            var resultData = _gronsfeldService.Encrypt(encryptData, key);
            _logger.LogWarning($"Данные отправляемые клиенту:{resultData}");
            context.Result = new ContentResult()
            {
                Content = resultData,
                StatusCode = 200,
                ContentType = "text/plain"
            };
            _logger.LogInformation($"Конец результата");
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}