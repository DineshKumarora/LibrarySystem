
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Steeltoe.Discovery.Client;

namespace APIGateway
{    
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Ocelot.DependencyInjection;
    using Ocelot.Middleware;
    using System;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Logging;
    using NLog.Extensions.Logging;
    using Swashbuckle.AspNetCore.Swagger;

    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Start Up
        /// </summary>
        /// <param name="env">Host Environment</param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
            builder.SetBasePath(env.ContentRootPath)
                   .AddJsonFile($"appsettings.json")
                   .AddJsonFile("configuration.json", optional: false, reloadOnChange: true)
                   .AddEnvironmentVariables();

            NLog.LogManager.LoadConfiguration(Path.Combine(env.ContentRootPath, "nlog.config")); // For Logging
            Configuration = builder.Build();

            //For Limit of calls or whilelisting and other features changes done in Configuration file
            //For Logging refer the comments in this file

            //Quality of services changes done in Configuration
            //If the server does not response for 2 seconds, it will throw a timeout exception.
            //If the server throws a second exception, the server will not be accessible for five seconds
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfigurationRoot Configuration { get; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            var audienceConfig = Configuration.GetSection("Audience");

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(audienceConfig["Secret"]));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Iss"],
                ValidateAudience = true,
                ValidAudience = audienceConfig["Aud"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = "TestKey";
            })
            .AddJwtBearer("TestKey", x =>
             {
                 x.RequireHttpsMetadata = false;
                 x.TokenValidationParameters = tokenValidationParameters;
             });
            services.AddMvcCore().AddApiExplorer();
            services.AddOcelot(Configuration);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIGateway", Version = "v1" });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "APIGateway.xml");
                c.IncludeXmlComments(xmlPath);
            });
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            //services.AddDiscoveryClient(Configuration);
        }

        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //for Logging and parameter in te signature,
            //Logging section in appsettings and nlog config file
            //in startup method NLog.LogManager.LoadConfiguration("nlog.config"); 
            //in program file .UseNLog() mehotd calling
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            app.UseSwagger(c =>
            {
            });
            app.UseSwagger().UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIGateway");
                c.SwaggerEndpoint("/a/swagger.json", "BooksAPIServices");
                c.SwaggerEndpoint("/b/swagger.json", "SubscriptionAPIService");
                c.SwaggerEndpoint("/c/swagger.json", "CustomerAPIService");
                c.SwaggerEndpoint("/d/swagger.json", "AlertSubscriptionService");
            });
            //app.UseDiscoveryClient();
            app.UseAuthentication();
            await app.UseOcelot();
        }
    }
}
