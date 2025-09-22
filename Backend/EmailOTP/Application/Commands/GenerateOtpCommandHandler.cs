using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

public class GenerateOtpCommandHandler : IRequestHandler<GenerateOtpCommand, HandlerResponse>
{
    private readonly IOtpRepository _otpRepository;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<GenerateOtpCommandHandler> _logger;
    
    private const string EmailDomain = "dso.org.sg";

    public GenerateOtpCommandHandler(IOtpRepository otpRepository, IEmailSender emailSender, ILogger<GenerateOtpCommandHandler> logger)
    {
        _otpRepository = otpRepository;
        _emailSender = emailSender;
        _logger = logger;
    }
    
    public async Task<HandlerResponse> Handle(GenerateOtpCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email;
        var splits = email.Trim().Split('@');

        if (splits.Length != 2)
        {
            _logger.LogError("Invalid email format");
            return HandlerResponse.Failed(OtpStatus.InvalidEmail, "Invalid email format");
        }
        
        var domain = splits[1];

        if (domain != EmailDomain)
        {
            _logger.LogError("Invalid email domain");
            return HandlerResponse.Failed(OtpStatus.InvalidEmailDomain, "Invalid email domain");
        }
        
        (string otp, string otpHash) = GenerateOtp();
        
        string subject = "Your OTP has been generated";
        string body = $"Your OTP is {otp}";
        bool isSuccess = await _emailSender.SendEmailAsync(email, subject, body);

        if (!isSuccess)
        {
            _logger.LogError("Sending OTP email failed");
            return HandlerResponse.Failed(OtpStatus.EmailFailed, "Sending OTP email failed");
        }
        _logger.LogInformation("Sending OTP email succeeded");
        

        var otpRecord = new OtpMessage
        {
            Email = email,
            OtpHash = otpHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(1),
            AttemptCount = 0,
        };
        await _otpRepository.AddAsync(otpRecord);
        _logger.LogInformation("OTP record created in the database");
        
        return HandlerResponse.Success();
    }

    private static (string otp, string otpHash) GenerateOtp()
    {
        var random = new Random();
        string otp = random.Next(0, 1000000).ToString("D6");
        string otpHash = BCrypt.Net.BCrypt.HashPassword(otp);
        
        return (otp, otpHash);
    }
}