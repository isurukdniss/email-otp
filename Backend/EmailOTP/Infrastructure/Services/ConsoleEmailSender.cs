using Application.Interfaces;

namespace Infrastructure.Services;

public class ConsoleEmailSender : IEmailSender
{
    // Simulating the email sending functionality
    public Task<bool> SendEmailAsync(string email, string subject, string message)
    {
        Console.WriteLine($"Sending email to {email}...");
        Console.WriteLine("Email sent");
        return Task.FromResult(true);
    }
}