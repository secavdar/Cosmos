using Cosmos.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.Middleware
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;

        public LogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableRewind();

            var apiLogEntry = CreateApiLogEntryWithRequestData(context);

            var bodyStream = context.Response.Body;
            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await _next(context);

            responseBodyStream.Seek(0, SeekOrigin.Begin);

            apiLogEntry.ResponseContentBody = new StreamReader(responseBodyStream).ReadToEnd();
            apiLogEntry.ResponseContentType = context.Response.ContentType;
            apiLogEntry.ResponseHeaders = SerializeHeaders(context.Response.Headers);
            apiLogEntry.ResponseTimestamp = DateTime.Now;
            apiLogEntry.ResponseStatusCode = context.Response.StatusCode.ToString();

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(bodyStream);

            //PushLog(apiLogEntry);
        }
        private LogEntry CreateApiLogEntryWithRequestData(HttpContext context)
        {
            var requestContentBody = new StreamReader(context.Request.Body).ReadToEnd();
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            var transactionId = context.Request.Headers["TransactionId"].FirstOrDefault() != null
                              ? context.Request.Headers["TransactionId"].FirstOrDefault()
                              : "";
            var clientIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() != null
                         ? context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                         : null;

            return new LogEntry
            {
                AppUser = context.User?.Identity?.Name,
                Machine = Environment.MachineName,
                RequestContentType = context.Request?.ContentType,
                RequestIpAddress = clientIp ?? context.Connection.RemoteIpAddress.ToString(),
                RequestMethod = context.Request.Method,
                RequestHeaders = SerializeHeaders(context.Request.Headers),
                RequestContentBody = requestContentBody,
                RequestTimestamp = DateTime.Now,
                RequestUri = context.Request.GetDisplayUrl(),
            };
        }
        private string SerializeHeaders(IHeaderDictionary headers)
        {
            var dict = headers.Where(x => x.Key != "TransactionId" && x.Key != "X-Forwarded-For")
                              .ToDictionary(x => x.Key, y => String.Join(" ", y.Value));

            return JsonConvert.SerializeObject(dict, Formatting.Indented);
        }
    }
}