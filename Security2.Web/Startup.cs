using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Security2.CustomAuth;
using Security2.Database;
using Security2.Domain.Utils;
using Swashbuckle.AspNetCore.Swagger;

namespace Security2.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        
        private IHostingEnvironment _hostingEnvironment;
        
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config =>
            {
                //config.OutputFormatters.Insert(0, new CustomOutputFormatter());
            });

            services.AddAuthorization(options =>
            {
                //options.AddPolicy(
                //    TestAuthOptions.DefaultScheme,
                //    policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Email));
            });
            //services.AddAuthentication(TestAuthOptions.DefaultScheme)
            //.AddTestAuth(opt => { });

            services.AddDbContext<DatabaseContext>(opt =>
                opt.UseNpgsql(Configuration["Database:ConnectionString"],
                    npgOpt => npgOpt.MigrationsAssembly("Security2.Web")));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.AddDomainServices(Configuration);
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new Info { Title = "Документация API", Version = "v1" });
                
                opt.DescribeAllEnumsAsStrings();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(a =>
            {
                a.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
                a.RoutePrefix = "api/help";
            });

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }

    }

    //public class CustomOutputFormatter : TextOutputFormatter
    //{
    //    public CustomOutputFormatter()
    //    {
    //        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));

    //        SupportedEncodings.Add(Encoding.UTF8);
    //        SupportedEncodings.Add(Encoding.Unicode);
    //    }

    //    /// <inheritdoc />
    //    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    //    {

    //    }
    //}
}