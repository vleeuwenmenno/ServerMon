using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ServerMon.Helpers.Authorization
{
    public static class ApiKeyMiddleWareExtension
    {
        public static IApplicationBuilder UseApiKeys(  
            this IApplicationBuilder app, IFreeSql database)  
        {  
            return app.UseMiddleware<ApiKeyMiddleware>(database);  
        }  
    }

    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IFreeSql _db;
        private const string ApiKeyHeader = "X-Api-Key";
        
        public ApiKeyMiddleware(  
            RequestDelegate next,  
            IFreeSql options)  
        {  
            this._next = next;  
            this._db = options;  
        } 

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out StringValues extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key was not provided.");
                return;
            }

            if (!Authentication.VerifyAPIAccess(extractedApiKey[0], this._db))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync
                      ("Unauthorized.");
                return;
            }

            await _next(context);
        }
    }
}