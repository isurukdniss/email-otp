using Application.Common;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, HandlerResponse>
{
    private const int MaxAllowedAttempts = 10;
    
    private readonly IOtpRepository _otpRepository;
    private readonly ILogger<VerifyOtpCommandHandler> _logger;

    public VerifyOtpCommandHandler(IOtpRepository otpRepository, ILogger<VerifyOtpCommandHandler> logger)
    {
        _otpRepository = otpRepository;
        _logger = logger;
    }
    
    public async Task<HandlerResponse> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {

        var savedOtp = await _otpRepository.GetOtpByEmailAsync(request.Email);

        if (savedOtp is null)
        {
            _logger.LogError("Couldn't find OTP with email {Email}", request.Email);
            return HandlerResponse.Failed(OtpStatus.NotFound, "You haven't registered an OTP.");
        }
        
        if (savedOtp.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogError("The OTP has expired");
            return HandlerResponse.Failed(OtpStatus.Expired, "The OTP has expired.");
        }
        
        if (savedOtp.AttemptCount >= MaxAllowedAttempts)
        {
            _logger.LogError("Exceeded maximum attempts");
            return HandlerResponse.Failed(OtpStatus.TooManyAttempts, "Too many attempts.");
        }
        
        var isValidOtp = BCrypt.Net.BCrypt.Verify(request.Otp, savedOtp.OtpHash);

        if (!isValidOtp)
        {
            savedOtp.AttemptCount++;
            await _otpRepository.UpdateAsync(savedOtp);
            
            int attemptsRemaining = MaxAllowedAttempts - savedOtp.AttemptCount;
            
            _logger.LogError("Invalid OTP. Attempts remaining: {AttemptsRemaining}", attemptsRemaining);
            return HandlerResponse.Failed(OtpStatus.InvalidOtp, $"Invalid OTP. You have {attemptsRemaining} attempts remaining.");
        }
        
        savedOtp.VerifiedAt = DateTime.UtcNow;
        await _otpRepository.UpdateAsync(savedOtp);
        
        _logger.LogInformation("OTP verified and updated the database");
        
        return HandlerResponse.Success();
    }
}