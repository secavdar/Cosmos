using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.Middleware
{
    public class TransactionMiddleware
    {
        private readonly RequestDelegate _next;

        public TransactionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var result = context.Request.Headers["TransactionId"].FirstOrDefault();

            if (result == null)
                context.Request.Headers.Add("TransactionId", Guid.NewGuid().ToString());

            await _next(context);
        }
    }
}