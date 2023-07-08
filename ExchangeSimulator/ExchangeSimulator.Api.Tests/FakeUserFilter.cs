using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExchangeSimulator.Api.Tests;

public class FakeUserFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var claimsPrincipal = new ClaimsPrincipal();

        claimsPrincipal.AddIdentity(new ClaimsIdentity(

            new[]
            {
                    new Claim(ClaimTypes.NameIdentifier,Constants.UserId),
                    new Claim(ClaimTypes.Role,"User"),
                    new Claim("IsVerified", true.ToString())
            }));
        context.HttpContext.User = claimsPrincipal;

        await next();
    }
}