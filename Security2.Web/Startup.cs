using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Security2.Database;
using Security2.Domain.Utils;
using Security2.Rsa;
using Security2.Web.Utils;
using Swashbuckle.AspNetCore.Swagger;

namespace Security2.Web
{
    /// <summary>
    /// Настройка сервера
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        
        private IHostingEnvironment _hostingEnvironment;
        
        //Настройка DI        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config =>
            {
                //config.OutputFormatters.Insert(0, new CustomOutputFormatter());
                config.Filters.Add<ErrorHandler>();
            });

            services.AddAuthorization(options =>
            {
            });


            services.AddDbContext<DatabaseContext>(opt =>
                opt.UseNpgsql(Configuration["Database:ConnectionString"],
                    npgOpt => npgOpt.MigrationsAssembly("Security2.Web")));
            services.AddMemoryCache();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,opt =>
                {
                    opt.Cookie.HttpOnly = false;
                });
            services.AddRsaService();


            services.AddDomainServices(Configuration);
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new Info { Title = "Документация API", Version = "v1" });
                
                opt.DescribeAllEnumsAsStrings();
            });

            var rsaKeys  = RsaService.GetKeyPair(8184);
            var serverKeys = new RsaServerKeys(rsaKeys.PublicKey, rsaKeys.PrivateKey);

            services.AddSingleton(serverKeys);

        }
        
        /// <summary>
        /// Настройка путей и pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(a =>
            {
                a.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
                a.RoutePrefix = "api/help";
            });
            
            app.UseMvcWithDefaultRoute();
        }
    }
}