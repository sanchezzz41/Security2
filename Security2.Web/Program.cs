using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Security2.Database;

namespace Security2.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var build = CreateWebHostBuilder(args).Build();
            build.Services.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>().Database
                .Migrate();
            build.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}