using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Security2.Rsa
{
    public static class RsaExtensions
    {
        public static string RsaName = "CustomRsa";

        public static IServiceCollection AddRsaService(this IServiceCollection services)
        {
            services.AddScoped<RsaService>();
            return services;
        }


        public static string GetUserId(string email)
        {
            return email + RsaName;
        }
    }
}
