namespace UniParque_Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string JwtId { get; set; }
    public string UserId { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? ReplaceByJwtId { get; set; }

    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive  =>  !IsRevoked && !IsExpired;
}
