using UniParque_Domain.Entities;

namespace UniParque.Application.Repositories;

public interface IPaymentRepository
{
    Task<Payment> AddAsync(Payment payment);
}
