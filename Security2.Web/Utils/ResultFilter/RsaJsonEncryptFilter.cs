using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Security2.Rsa;

namespace Security2.Web.Utils.ResultFilter
{
    /// <summary>
    /// Фильтр для результат, шифрующий в RSA
    /// </summary>
    public class RsaJsonEncryptFilter : Attribute, IResultFilter
    {
        private ILogger _logger;
        private RsaService _rsaService;
        private IMemoryCache _memoryCache;

        /// <inheritdoc />
        public void OnResultExecuting(ResultExecutingContext context)
        {
            var serviceProviders = context.HttpContext.RequestServices.CreateScope().ServiceProvider;
            _logger = serviceProviders.GetService<ILoggerProvider>()
                .CreateLogger("RsaJsonEncryptFilter");
            _rsaService = serviceProviders.GetService<RsaService>();
            _memoryCache = serviceProviders.GetService<IMemoryCache>();
            _logger.LogInformation($"RSA");

            var objResult = (ObjectResult)context.Result;
            var email = context.HttpContext.GetEmail();
            var key = _memoryCache.Get<RsaPublicKey>(RsaExtensions.GetUserId(email));
            var resultData = _rsaService.Encrypt(objResult.Value, key);
            context.Result = new ContentResult()
            {
                Content = resultData,
                StatusCode = 200,
                ContentType = "text/plain"
            };
            _logger.LogInformation($"Конец результата");
        }

        /// <inheritdoc />
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
