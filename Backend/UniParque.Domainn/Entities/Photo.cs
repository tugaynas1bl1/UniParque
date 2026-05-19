namespace UniParque_Domain.Entities;

public class Photo
{
    public Guid Id { get; set; }
    public string Url { get; set; }
    public string PublicId { get; set; }
    public AppUser User { get; set; }
    public string UserId { get; set; }
}
