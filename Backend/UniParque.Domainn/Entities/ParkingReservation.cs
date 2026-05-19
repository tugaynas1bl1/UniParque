namespace UniParque_Domain.Entities;

public class ParkingReservation
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public AppUser User { get; set; }
    public Guid PlaceId { get; set; }
    public ParkingPlace Place { get; set; }
    public string CarNumber { get; set; }
    public DateTimeOffset? EstimatedArrivalTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public decimal TotalPrice { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;
    public DateTimeOffset? ReservationDeletionTime { get; set; } = null;
}
