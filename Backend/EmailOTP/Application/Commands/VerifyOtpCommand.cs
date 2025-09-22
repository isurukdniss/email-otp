using Application.Common;
using MediatR;

namespace Application.Commands;

public class VerifyOtpCommand : IRequest<HandlerResponse>
{
    public required string Email { get; set; }
    public required string Otp { get; set; }
}