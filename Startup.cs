using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ServerMon.Helpers;
using ServerMon.Helpers.Authorization;
using ServerMon.Helpers.Filters;

namespace ServerMon
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IFreeSql db { get; }
        public Options options {get;}

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            options = Options.LoadOptions();
            db = Database.loadDatabase(this.options);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddControllers();

            // Generate swagger files for the project
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "ServerMon", 
                    Version = $"{Program.programVersion}",
                    Description = "API Calls for retrieving info from ServerMon logging."
                });

                c.OperationFilter<DefaultHeaderFilter>();
            });

            // Enable IP Rate limiting
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            // Enable memory cachcing for IP rate limiting
            services.AddMemoryCache();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();

            // Add database and options to services so we can access this in any controller
            services.AddSingleton<IFreeSql>(db);
            services.AddSingleton<Options>(options);
            
            // Make sure we support reverse proxy
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
                            IWebHostEnvironment env,
                            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("logs/api-{Date}.log");
        
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"ServerMon {Program.programVersion}"));
            }

            // Enable the use of IP rate limiting
	        app.UseIpRateLimiting();

            // Make sure we support reverse proxy
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            
            app.UseRouting();
            app.UseAuthorization();
            app.UseApiKeys(db);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
