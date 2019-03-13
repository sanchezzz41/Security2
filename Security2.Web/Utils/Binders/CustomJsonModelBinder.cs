using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Security2.Domain.Services;
using Security2.Domain.Utils;

namespace Security2.Web.Binders
{
    public class CustomJsonModelBinder : IModelBinder
    {
        private readonly GronsfeldService _gronsfeldService;
        private readonly ILogger<CustomJsonModelBinder> _logger;

        public CustomJsonModelBinder(GronsfeldService gronsfeldService,
            ILogger<CustomJsonModelBinder> logger)
        {
            _gronsfeldService = gronsfeldService;
            _logger = logger;
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

            var strSteam = new StreamReader(request.Body);
            var data = await strSteam.ReadToEndAsync();
            var key = bindingContext.HttpContext.User.FindFirst(KeyGenerator.ClaimType).Value;
            //TODO Тут дешифруем
            //var resultModel = _gronsfeldService.Decrypt(data, key);
            //var test = JsonConvert.DeserializeObject(resultModel, type);
            //bindingContext.Result = ModelBindingResult.Success(test);
            var test = JsonConvert.DeserializeObject(data, type);
            //var test2 = _gronsfeldService.Encrypt(JsonConvert.SerializeObject(test), "123");
            //var test3 = _gronsfeldService.Decrypt(test2,
            //    "123");
            //test = JsonConvert.DeserializeObject(test3, type);

            _logger.LogInformation($"Входные данные:{data}\nКлюч пользователя:{key}\nРезультат дешифровки:{test}");
            bindingContext.Result = ModelBindingResult.Success(test);
        }
    }
}
