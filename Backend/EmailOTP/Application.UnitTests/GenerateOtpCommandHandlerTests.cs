using Application.Commands;
using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTests;

public class GenerateOtpCommandHandlerTests
{
    private readonly Mock<IOtpRepository> _mockOtpRepository;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly GenerateOtpCommandHandler _handler;

    public GenerateOtpCommandHandlerTests()
    {
        _mockOtpRepository = new Mock<IOtpRepository>();
        _mockEmailSender = new Mock<IEmailSender>();
        var mockLogger = new Mock<ILogger<GenerateOtpCommandHandler>>();
        _handler = new GenerateOtpCommandHandler(_mockOtpRepository.Object, _mockEmailSender.Object, mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WithValidEmail_ShouldReturnOkAndSendEmail()
    {
        // Arrange
        var email = "test1@dso.org.sg";
        var command = new GenerateOtpCommand { Email = email };
        
        _mockEmailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(response.IsSuccess);
        Assert.Empty(response.ErrorMessage);
        Assert.Equal(OtpStatus.Ok, response.StatusCode);
        
        _mockOtpRepository.Verify(r => r.AddAsync(It.Is<OtpMessage>(otp => otp.Email == email))
            , Times.Once);
        _mockEmailSender.Verify(sender => sender.SendEmailAsync(
                email, 
                It.Is<string>(subject => subject.Contains("Your OTP has been generated")),
                It.Is<string>(body => body.Contains("Your OTP is")))
        , Times.Once);
        
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldReturnErrorAndNotSendEmail()
    {
        // Arrange
        var email = "test1@example.com";
        var command = new GenerateOtpCommand { Email = email };
        
        // Act
        var response = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.False(response.IsSuccess);
        Assert.NotEmpty(response.ErrorMessage);
        
        _mockOtpRepository.Verify(
            r => r.AddAsync(It.IsAny<OtpMessage>()), Times.Never);
        _mockEmailSender.Verify(sender => sender.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
            , Times.Never);
    }
    
}