using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.Middleware
{
    public class PreflightMiddleware
    {
        private readonly RequestDelegate _next;

        public PreflightMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.Headers.ContainsKey("Origin"))
            {
                var origin = request.Headers["Origin"].FirstOrDefault();

                if (request.Method.Equals("OPTIONS"))
                {
                    response.StatusCode = 200;

                    response.Headers.Add("Access-Control-Allow-Origin", origin);
                    response.Headers.Add("Access-Control-Allow-Headers", "origin, content-type, accept");
                    response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE");
                    response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    response.Headers.Add("Access-Control-Max-Age", "600");
                }
                else
                {
                    response.OnStarting(() =>
                    {
                        response.Headers.Add("Access-Control-Allow-Origin", origin);
                        response.Headers.Add("Access-Control-Allow-Headers", "origin, content-type, accept");
                        response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE");
                        response.Headers.Add("Access-Control-Allow-Credentials", "true");
                        response.Headers.Add("Access-Control-Max-Age", "600");

                        return Task.CompletedTask;
                    });

                    await _next(context);
                }
            }
            else
                await _next(context);
        }
    }
}