using System;
using Microsoft.AspNetCore.Builder;

namespace MiddlewareTest.Controllers
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app) {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
