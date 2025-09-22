using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OtpRepository : IOtpRepository
{
    private readonly ApplicationDbContext _context;

    public OtpRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<OtpMessage?> GetOtpByEmailAsync(string email)
    {
        return await _context.OtpMessages
            .Where(o => o.Email == email && o.VerifiedAt == null)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(OtpMessage otpMessage)
    {
        _context.OtpMessages.Add(otpMessage);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(OtpMessage otpMessage)
    {
        _context.OtpMessages.Update(otpMessage);
        await _context.SaveChangesAsync();
    }
}