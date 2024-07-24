using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class FunctionLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public FunctionLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine($"Request Path: {context.Request.Path}");
            await _next(context);
        }
    }
}
