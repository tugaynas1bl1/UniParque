using UniParque.Application.Repositories;
using UniParque_Domain.Entities;
using UniParque_Infrastructure.Persistence;

namespace UniParque_Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly UniParqueDBContext _context;

    public PaymentRepository(UniParqueDBContext context)
        => _context = context;

    public async Task<Payment> AddAsync(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        await _context.Entry(payment).Reference(p => p.User).LoadAsync();
        return payment;
    }
}
