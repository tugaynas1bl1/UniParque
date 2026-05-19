using UniParque.Application.DTOs;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;

namespace UniParque.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IPaymentRepository paymentRepository)
        =>  _paymentRepository = paymentRepository;
    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        var paymentResult = new PaymentResult
        {
            IsSuccess = true,
            PaymentDate = DateTimeOffset.UtcNow,
        };

        if (paymentResult.IsSuccess)
        {
            var payment = new Payment
            {
                CardNumber = request.CardNumber,
                Amount = request.Amount,
                PaymentDate = paymentResult.PaymentDate,
                Status = PaymentStatus.Paid,
                UserId = request.UserId
            };

            paymentResult.PaymentId = payment.Id;

            await _paymentRepository.AddAsync(payment);
        }

        return paymentResult;
    }
}
