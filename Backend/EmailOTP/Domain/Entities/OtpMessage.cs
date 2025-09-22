using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class OtpMessage
{
    [Key]
    public Guid Guid { get; set; }
    public required string Email { get; set; }
    public required string OtpHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public int AttemptCount { get; set; }
}