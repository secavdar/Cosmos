using Cosmos.Helper;
using Cosmos.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Cosmos.Extension
{
    public static class LogMiddlewareExtension
    {
        public static IApplicationBuilder UseApiLogMiddleware(this IApplicationBuilder app)
        {
            var serviceCollection = app.ApplicationServices.GetService<IServiceCollection>();
            serviceCollection.AddSingleton<ProxyHelper>();

            app.UseMiddleware<TransactionMiddleware>();
            app.UseMiddleware<LogMiddleware>();

            return app;
        }
    }
}