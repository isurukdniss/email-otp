using Domain.Entities;

namespace Application.Interfaces;

public interface IOtpRepository
{
    Task<OtpMessage?> GetOtpByEmailAsync(string email);
    Task AddAsync(OtpMessage otpMessage);
    Task UpdateAsync(OtpMessage otpMessage);
}