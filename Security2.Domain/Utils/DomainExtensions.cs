using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Security2.Database.Entities;
using Security2.Domain.Services;
using Security2.Gronsfer;

namespace Security2.Domain.Utils
{
    public static class DomainExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration conf)
        {
            services.AddScoped<UserAccount>();
            services.AddScoped<NewsService>();
            services.AddScoped<GronfeldEncrypt>();
            services.AddScoped<GronsfeldService>();
            services.AddScoped<KeyGenerator>();

            var opt = new GronsfeldOptions
            {
                Alp = conf["Gronsfeld:alp"],
                KeyLenght = int.Parse(conf["Gronsfeld:keyLenght"])
            };
            services.AddSingleton(opt);
            
            return services;
        }
        
    }
}