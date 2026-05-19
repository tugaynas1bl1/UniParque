using Microsoft.EntityFrameworkCore;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;
using UniParque_Infrastructure.Persistence;

namespace UniParque_Infrastructure.Repositories;

public class UserVerificationRepository : IUserVerificationRepository
{
    private readonly UniParqueDBContext _context;

    public UserVerificationRepository(UniParqueDBContext context)
        =>  _context = context;

    public async Task<UserVerificationCode> AddAsync(UserVerificationCode verificationCode)
    {
        _context.UserVerificationCodes.Add(verificationCode);
        await _context.Entry(verificationCode).Reference(cv => cv.User).LoadAsync();
        await _context.SaveChangesAsync();
        return verificationCode;
    }

    public async Task ChangeCodeAsync(UserVerificationCode verificationCode, int code)
    {
        verificationCode.Code = code;
        verificationCode.ExpiresAt = DateTime.UtcNow.AddMinutes(3);
        await _context.SaveChangesAsync();
    }

    public async Task<UserVerificationCode> GetByUserId(string userId)
    {
        var code = await _context
                        .UserVerificationCodes
                        .Include(vc => vc.User)
                        .FirstOrDefaultAsync(vc => vc.UserId == userId);
        return code!;
    }

    public async Task RemoveAsync(UserVerificationCode verificationCode)
    {
        _context.UserVerificationCodes.Remove(verificationCode);
        await _context.SaveChangesAsync();
    }
}
