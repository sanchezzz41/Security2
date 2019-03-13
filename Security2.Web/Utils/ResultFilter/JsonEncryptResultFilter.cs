using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Security2.Domain.Services;
using Security2.Domain.Utils;

namespace Security2.Web.ResultFilter
{
    public class JsonEncryptResultFilter : Attribute, IResultFilter
    {
        private ILogger _logger;
        private GronsfeldService _gronsfeldService;

        public void OnResultExecuting(ResultExecutingContext context)
        {
            var serviceProviders = context.HttpContext.RequestServices.CreateScope().ServiceProvider;
            _logger = serviceProviders.GetService<ILoggerProvider>()
                .CreateLogger("JsonEncryptResultFilter");
            _gronsfeldService = serviceProviders.GetService<GronsfeldService>();
            var objResult = (ObjectResult) context.Result;
            var key = context.HttpContext.User.FindFirst(KeyGenerator.ClaimType).Value;
            var encryptData = JsonConvert.SerializeObject(objResult.Value);
            var resultData = _gronsfeldService.Encrypt(encryptData, key);
            var test = _gronsfeldService.Decrypt(resultData, key);
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