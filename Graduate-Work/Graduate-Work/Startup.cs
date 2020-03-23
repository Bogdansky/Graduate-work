using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Business_Logic_Layer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Business_Logic_Layer.Services.Crud;
using Business_Logic_Layer.Profiles;

namespace Graduate_Work
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
            services.AddSwaggerGen(c => 
            { 
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task manager", Version = "v1" }); 
            });

            services.AddControllers();
            services.AddAutoMapper(typeof(TaskProfile));

            services.AddTransient<AccountService>();
            services.AddTransient<UserService>();
            services.AddScoped<ContextFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            var logger = loggerFactory.CreateLogger<Startup>();
            app.Run(async (context) =>
            {
                logger.LogInformation("Requested Path: {0}", context.Request.Path);
                await context.Response.WriteAsync("Hello World!");
            });
            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task manager - V1");
                c.RoutePrefix = string.Empty; //To serve the Swagger UI at the app's root (http://localhost:<port>/), set the RoutePrefix property to an empty string
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
