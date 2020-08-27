using System.IO;
using SubscriptionAPIService.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Steeltoe.Discovery.Client;
using Microsoft.OpenApi.Models;
//using SubscriptionAPIService.Service;

namespace SubscriptionAPIService
{
    using System;
    using System.Text;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Swashbuckle.AspNetCore.Swagger;
    using Microsoft.Extensions.PlatformAbstractions;

    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

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

            services.AddDiscoveryClient(Configuration);
            services.AddMvc();
            ////request Header
            //services.AddTransient<RequestHandler>();
            //services.AddHttpClient<TargetClient>((client) => client.BaseAddress = new System.Uri("http://localhost:62793"))
            //.AddHttpMessageHandler<RequestHandler>(); //Override SendAsync method 
            //services.AddHttpContextAccessor();  //For http request context accessing
            ////services.AddTransient<ICorrelationIdAccessor, CorrelationIdAccessor>();  //DI


            //Swagger Changes
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SubscriptionAPIService", Version = "v1" });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "SubscriptionAPIService.xml");
                c.IncludeXmlComments(xmlPath);
            });
            services.AddMvcCore().AddApiExplorer();
            services.AddDbContext<SubscriptionContext>(opt =>
                opt.UseInMemoryDatabase("SubscriptionList"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDiscoveryClient();
            app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger(c =>
            {
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SubscriptionAPIService");
            });
        }
    }
}
