namespace UniParque_Domain.Entities;

public class ParkingBranch
{
    public Guid Id { get; set; }
    public string BranchName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; } = null;

    public ICollection<ParkingSection> Sections { get; set; }
        = new List<ParkingSection>();
}
