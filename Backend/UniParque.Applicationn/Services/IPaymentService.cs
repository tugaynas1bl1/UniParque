using UniParque.Application.DTOs;

namespace UniParque.Application.Services;


public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
}
