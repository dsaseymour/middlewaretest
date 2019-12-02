using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace MiddlewareTest.Controllers
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IActionResultExecutor<ObjectResult> executor;
        private static readonly ActionDescriptor EmptyActionDescriptor = new ActionDescriptor();

        public ExceptionMiddleware(RequestDelegate next, IActionResultExecutor<ObjectResult> executor, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.executor = executor;

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Console()
               .WriteTo.File("logs\\myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"An unhandled exception has occurred while executing the request. Url: {context.Request.GetDisplayUrl()}. Request Data: " + GetRequestData(context));

                if (context.Response.HasStarted)
                {
                    throw;
                }

                var routeData = context.GetRouteData() ?? new RouteData();

                ClearCacheHeaders(context.Response);

                var actionContext = new ActionContext(context, routeData, EmptyActionDescriptor);

                ObjectResult result = new ObjectResult(new ErrorResponse("Error processing request. Server error."))
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                };

                await executor.ExecuteAsync(actionContext, result);
            }
        }

        private static string GetRequestData(HttpContext context)
        {
            StringBuilder sb = new StringBuilder();

            if (context.Request.HasFormContentType && context.Request.Form.Any())
            {
                sb.Append("Form variables:");
                foreach (var x in context.Request.Form)
                {
                    sb.AppendFormat("Key={0}, Value={1}<br/>", x.Key, x.Value);
                }
            }

            sb.AppendLine("Method: " + context.Request.Method);

            return sb.ToString();
        }

        private static void ClearCacheHeaders(HttpResponse response)
        {
            response.Headers[HeaderNames.CacheControl] = "no-cache";
            response.Headers[HeaderNames.Pragma] = "no-cache";
            response.Headers[HeaderNames.Expires] = "-1";
            response.Headers.Remove(HeaderNames.ETag);
        }

        [DataContract(Name = "ErrorResponse")]
        public class ErrorResponse
        {
            [DataMember(Name = "Message")]
            public string Message { get; set; }

            public ErrorResponse(string message)
            {
                Message = message;
            }
        }
    }


}
