using ExchangeSimulator.Application.Requests.RegisterUser;
using ExchangeSimulator.Application.Requests.SignIn;
using ExchangeSimulator.Application.Requests.VerifyEmail;
using ExchangeSimulator.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeSimulator.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISmtpService _smtpService;

    public UserController(IMediator mediator, ISmtpService smtpService)
    {
        _mediator = mediator;
        _smtpService = smtpService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
    {
        await _mediator.Send(request);
        return Ok();
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn(SignInRequest request)
    {
        var token = await _mediator.Send(request);
        return Ok(token);
    }

    [HttpPut("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request) { 
        await _mediator.Send(request);
        return Ok();
    }

}
