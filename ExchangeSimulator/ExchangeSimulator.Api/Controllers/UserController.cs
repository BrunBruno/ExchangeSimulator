using ExchangeSimulator.Application.Requests.UserRequests.IsEmailVerified;
using ExchangeSimulator.Application.Requests.UserRequests.RegenerateEmailVerificationCode;
using ExchangeSimulator.Application.Requests.UserRequests.RegisterUser;
using ExchangeSimulator.Application.Requests.UserRequests.SignIn;
using ExchangeSimulator.Application.Requests.UserRequests.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeSimulator.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers user and sends email verification code.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
    {
        await _mediator.Send(request);
        return Ok();
    }

    /// <summary>
    /// Creates Jwt token for user.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Jwt token.</returns>
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn(SignInRequest request)
    {
        var token = await _mediator.Send(request);
        return Ok(token);
    }

    /// <summary>
    /// Verifies email.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("verify-email")]
    [Authorize(Policy = "IsNotVerified")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request) 
    { 
        await _mediator.Send(request);
        return Ok();
    }

    /// <summary>
    /// Removes old code and creates new.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("regenerate-code")]
    [Authorize(Policy = "IsNotVerified")]
    public async Task<IActionResult> RegenerateCode()
    {
        var request = new RegenerateEmailVerificationCodeRequest();
        await _mediator.Send(request);
        return Ok();
    }

    /// <summary>
    /// Checks if email is verified.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Bool.</returns>
    [HttpGet("is-verified")]
    [Authorize]
    public async Task<IActionResult> IsEmailVerified()
    {
        var request = new IsEmailVerifiedRequest();
        var result = await _mediator.Send(request);
        return Ok(result);
    }
}