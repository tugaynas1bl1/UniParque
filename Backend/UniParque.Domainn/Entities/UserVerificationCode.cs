namespace UniParque_Domain.Entities;

public class UserVerificationCode
{
    public Guid Id { get; set; }
    public int Code { get; set; } = default;
    public DateTimeOffset ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(3);
    public string UserId { get; set; }
    public AppUser User { get; set; }
}
