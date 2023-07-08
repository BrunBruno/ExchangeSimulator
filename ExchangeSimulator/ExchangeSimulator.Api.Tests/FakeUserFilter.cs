using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExchangeSimulator.Api.Tests;

public class FakeUserFilter : IAsyncActionFilter
{
    private readonly bool _isVerified;

    public FakeUserFilter(bool isVerified)
    {
        _isVerified = isVerified;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var claimsPrincipal = new ClaimsPrincipal();

        claimsPrincipal.AddIdentity(new ClaimsIdentity(

            new[]
            {
                    new Claim(ClaimTypes.NameIdentifier,Constants.UserId),
                    new Claim(ClaimTypes.Role,"User"),
                    new Claim("IsVerified", _isVerified.ToString())
            }));
        context.HttpContext.User = claimsPrincipal;

        await next();
    }
}