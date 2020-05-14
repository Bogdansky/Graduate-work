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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Business_Logic_Layer.Services.Crud;
using Business_Logic_Layer.Profiles;
using Microsoft.AspNetCore.Diagnostics;
using System.IO;
using Graduate_Work.Hubs;
using Graduate_Work.Models.Interfaces;
using Graduate_Work.Models;
using Microsoft.AspNetCore.Http.Connections;

namespace Graduate_Work
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string MyAllow = "AllowOrigin";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var identitySection = Configuration.GetSection("AuthOptions");
            services.AddSwaggerGen(c => 
            { 
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task manager", Version = "v1" }); 
            });

            services.AddControllers();
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
            services.AddAutoMapper(typeof(TaskProfile));

            services.AddScoped<AccountService>();
            services.AddScoped<UserService>();
            services.AddScoped<ProjectService>();
            services.AddScoped<TaskService>();
            services.AddScoped<EmployeeService>();
            services.AddScoped<ContextFactory>();
            services.AddSingleton<EmailService>();
            services.AddSingleton<ServiceCache>();
            services.AddSingleton<IConnectionManager, ConnectionManager>();
            services.AddMemoryCache();
            services.AddLogging(cfg =>
            {
                cfg.AddConsole();
            });
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = identitySection.GetValue<string>("Issuer");
                options.RequireHttpsMetadata = false;
                options.Audience = identitySection.GetValue<string>("Audience");
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Authority,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    ValidateLifetime = true,
                };
            });

            services.AddCors(c => c.AddPolicy(MyAllow, builder =>
            {
                builder.WithOrigins(identitySection.GetValue<string>("Audience")).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync("{Error: \"Server was fell down\"}");
                });
            });
            app.UseHsts();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task manager - V1");
                c.RoutePrefix = string.Empty; //To serve the Swagger UI at the app's root (http://localhost:<port>/), set the RoutePrefix property to an empty string
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(MyAllow);

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapHub<TrackerHub>("/track", options =>
                {
                    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
                });
            });
        }
    }
}
