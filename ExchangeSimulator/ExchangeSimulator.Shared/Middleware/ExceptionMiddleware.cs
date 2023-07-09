using ExchangeSimulator.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace ExchangeSimulator.Shared.Middleware;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }

        catch (BadRequestException e)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(e.Message);
        }
        catch (NotFoundException e)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(e.Message);
        }

        catch (Exception e)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Something went wrong.");
        }
    }
}