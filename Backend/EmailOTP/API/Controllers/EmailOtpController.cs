using Application.Commands;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/otp")]
public class EmailOtpController : ControllerBase
{
    private readonly IMediator _mediator;
    public EmailOtpController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateOtp([FromBody] GenerateOtpCommand generateOtpCommand)
    {
        var response = await _mediator.Send(generateOtpCommand);

        if (response.IsSuccess)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand verifyOtpCommand)
    {
        var response = await _mediator.Send(verifyOtpCommand);

        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}