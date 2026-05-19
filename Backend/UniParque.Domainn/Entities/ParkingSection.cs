namespace UniParque_Domain.Entities;

public class ParkingSection
{
    public Guid Id { get; set; }
    public string Section { get; set; }
    public Guid BranchId { get; set; }
    public ParkingBranch? Branch { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; } = null;

    public ICollection<ParkingSubSection> SubSections { get; set; }
        = new List<ParkingSubSection>();
}
