namespace UniParque_Domain.Entities;

public class ParkingSubSection
{
    public Guid Id { get; set; }
    public string SubSection { get; set; }
    public Guid SectionId { get; set; }
    public ParkingSection? Section { get; set; } = null;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; } = null;

    public ICollection<ParkingPlace> Places { get; set; }
        = new List<ParkingPlace>();
}
