using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrReiz.AndroidGamer.Wui.Middlewares
{
  public class ErrorLoggingMiddleware
  {
    private readonly RequestDelegate _next;

    public ErrorLoggingMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (Exception e)
      {
        Log.Error(e, "Unhandled exception");
        throw;
      }
    }
  }

  public static class ErrorLoggingMiddlewareExtensions
  {
    public static IApplicationBuilder UseUnhandledErrorLogging(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<ErrorLoggingMiddleware>();
    }
  }
}
