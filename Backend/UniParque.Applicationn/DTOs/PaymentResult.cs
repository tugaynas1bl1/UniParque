namespace UniParque.Application.DTOs;

public class PaymentResult
{
    public bool IsSuccess { get; set; }

    public string? Message { get; set; }

    public DateTimeOffset PaymentDate { get; set; }

    public Guid? PaymentId { get; set; }
}
