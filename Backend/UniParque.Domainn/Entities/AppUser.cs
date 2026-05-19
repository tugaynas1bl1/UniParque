using Microsoft.AspNetCore.Identity;

namespace UniParque_Domain.Entities;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public decimal Balance { get; set; } = 0;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; } = null!;

    public ICollection<ParkingReservation> Reservations { get; set; }
        = new List<ParkingReservation>();

    public ICollection<Payment> Payments { get; set; }
        = new List<Payment>();

    public Photo? Photo { get; set; }
    public UserVerificationCode CodeVerification { get; set; }
}
