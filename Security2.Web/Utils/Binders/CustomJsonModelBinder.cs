using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Security2.Domain.Utils;
using Security2.Gronsfer;

namespace Security2.Web.Utils.Binders
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

            //var strSteam = new StreamReader(request.Body);
            var query = request.Query["data"].ToString().Replace(" ", "+");
            //var data = await strSteam.ReadToEndAsync();
            var key = bindingContext.HttpContext.User.FindFirst(KeyGenerator.ClaimType).Value;
            //TODO Тут дешифруем
            var decryptModel = _gronsfeldService.Decrypt(query, key);
            var resultData = JsonConvert.DeserializeObject(decryptModel, type);
            _logger.LogInformation($"Входные данные:{decryptModel}\nКлюч пользователя:{key}\nРезультат дешифровки:{JsonConvert.SerializeObject(resultData)}");
            bindingContext.Result = ModelBindingResult.Success(resultData);
        }
    }
}
