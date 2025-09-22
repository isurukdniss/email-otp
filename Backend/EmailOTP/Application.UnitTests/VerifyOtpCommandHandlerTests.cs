using Application.Commands;
using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTests;

public class VerifyOtpCommandHandlerTests
{
    private readonly Mock<IOtpRepository> _mockOtpRepository;
    private readonly VerifyOtpCommandHandler _handler;

    public VerifyOtpCommandHandlerTests()
    {
        _mockOtpRepository = new Mock<IOtpRepository>();
        var logger = new Mock<ILogger<VerifyOtpCommandHandler>>();
        _handler = new VerifyOtpCommandHandler(_mockOtpRepository.Object, logger.Object);
    }

    [Fact]
    public async Task Handle_WithValidOtp_ShouldReturnOk()
    {
        // Arrange
        var validOtp = "123456";
        var email = "abc@dso.org.sg";
        var command = new VerifyOtpCommand{ Email = email, Otp = validOtp };
        var otpMessage = new OtpMessage
        {
            Email = email,
            OtpHash = BCrypt.Net.BCrypt.HashPassword(validOtp),
            ExpiresAt = DateTime.UtcNow.AddMinutes(2),
            AttemptCount = 0
        };
        
        _mockOtpRepository.Setup(r => r.GetOtpByEmailAsync(command.Email)).ReturnsAsync(otpMessage);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(response.IsSuccess);
        Assert.NotNull(otpMessage.VerifiedAt);
        _mockOtpRepository.Verify(r => r.UpdateAsync(otpMessage), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidOtp_ShouldReturnError()
    {
        // Arrange
        var invalidOtp = "111111";
        var validOtp = "123456";
        var email = "abc@dso.org.sg";
        var command = new VerifyOtpCommand { Email = email, Otp = invalidOtp };
        var otpMessage = new OtpMessage
        {
            Email = email,
            OtpHash = BCrypt.Net.BCrypt.HashPassword(validOtp),
            ExpiresAt = DateTime.UtcNow.AddMinutes(2),
            AttemptCount = 0
        };
        
        _mockOtpRepository.Setup(r => r.GetOtpByEmailAsync(command.Email)).ReturnsAsync(otpMessage);
        
        // Act 
        var response = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.False(response.IsSuccess);
        Assert.Equal(OtpStatus.InvalidOtp, response.StatusCode);
        Assert.Equal(1, otpMessage.AttemptCount);
        _mockOtpRepository.Verify(r => r.UpdateAsync(otpMessage), Times.Once);
        
    }
}