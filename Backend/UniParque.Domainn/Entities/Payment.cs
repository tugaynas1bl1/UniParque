namespace UniParque_Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset PaymentDate { get; set; }
    public PaymentStatus Status { get; set; }
    public string UserId { get; set; }
    public AppUser User { get; set; }
}
