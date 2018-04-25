using Cosmos.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Cosmos.Extension
{
    public static class PreflightMiddlewareExtension
    {
        public static IApplicationBuilder UsePreflightMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<PreflightMiddleware>();
        }
    }
}