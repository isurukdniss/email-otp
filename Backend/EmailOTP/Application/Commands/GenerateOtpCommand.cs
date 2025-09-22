using Application.Common;
using MediatR;

namespace Application.Commands;

public class GenerateOtpCommand : IRequest<HandlerResponse>
{
    public required string Email { get; set; }
}