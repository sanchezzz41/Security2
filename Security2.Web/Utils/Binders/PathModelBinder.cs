using System;
using System.Linq;
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
    public class PathModelBinder : IModelBinder
    {
        private readonly GronsfeldService _gronsfeldService;
        private readonly ILogger<CustomJsonModelBinder> _logger;
        private readonly IMemoryCache _memoryCache;

        public PathModelBinder(GronsfeldService gronsfeldService,
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
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var type = bindingContext.ModelType;
            var pathValue = request.Query["id"].ToString().Replace(" ","+");
            if (pathValue == null)
                throw new NullReferenceException("Путь равен null.");
            //var key = bindingContext.HttpContext.User.FindFirst(KeyGenerator.ClaimType).Value;
            var key = _memoryCache.Get<string>(bindingContext.HttpContext.User.FindFirst(ClaimTypes.Email).Value);
            var decryptModel = _gronsfeldService.Decrypt(pathValue, key);
            _logger.LogInformation(
                $"Входные данные:{pathValue}\nКлюч пользователя:{key}\nРезультат дешифровки:{JsonConvert.SerializeObject(decryptModel)}");

            if (type == typeof(Guid))
            {
                bindingContext.Result = ModelBindingResult.Success(Guid.Parse(decryptModel));
                return;
            }

            if (type == typeof(int))
            {
                bindingContext.Result = ModelBindingResult.Success(int.Parse(decryptModel));
                return;
            }

            bindingContext.Result = ModelBindingResult.Success(decryptModel);
        }
    }
}
