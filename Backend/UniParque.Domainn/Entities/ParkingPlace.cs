namespace UniParque_Domain.Entities;

public class ParkingPlace
{
    public Guid Id { get; set; }
    public string PlaceName { get; set; }

    public Guid SubSectionId { get; set; }
    public ParkingSubSection? SubSection { get; set; } = null;
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; } = null!;

    public bool IsReserved { get; set; } = false;
    public bool IsTaken { get; set; } = false;

    public ICollection<ParkingReservation> Reservations { get; set; }
        = new List<ParkingReservation>();
}
