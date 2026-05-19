namespace UniParque.Application.DTOs;

public class PaymentRequest
{
    /// <summary>
    /// Card number used for the transaction
    /// </summary>
    /// <example>4098584487322842</example>
    public string CardNumber { get; set; }

    /// <summary>
    /// Amount of money
    /// </summary>
    /// <example>50.00</example>
    public decimal Amount { get; set; }

    /// <summary>
    /// Message (optional)
    /// </summary>
    /// <example>This money is for...</example>
    public string? Message { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    /// <example></example>
    public string UserId { get; set; }
}
