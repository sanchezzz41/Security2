﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;
using Security2.WebClient.RefitClients;
using Security2.WebClient.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace Security2.WebClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //services.AddRefitClient<IUserClient>()
            //    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5000/user"));
            services.AddHttpClient<UserService>(opt =>
                {
                    opt.BaseAddress = new Uri("http://localhost:5000/user");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new Info { Title = "Документация клиента", Version = "v1" });

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
            app.UseSwagger();
            app.UseSwaggerUI(a =>
            {
                a.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
                a.RoutePrefix = "api/help";
            });
            app.UseMvc();
        }
    }
}