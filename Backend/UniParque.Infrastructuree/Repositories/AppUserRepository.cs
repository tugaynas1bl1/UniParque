using Microsoft.EntityFrameworkCore;
using UniParque.Application.DTOs;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;
using UniParque_Infrastructure.Persistence;

namespace UniParque_Infrastructure.Repositories;

public class AppUserRepository : IAppUserRepository
{
    private readonly UniParqueDBContext _context;

    public AppUserRepository(UniParqueDBContext context)
        =>  _context = context;

    public async Task<AppUser> ChangeProfileDataAsync(AppUser user, EditProfileRequestDto request)
    {
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<AppUser> FindAsync(string userId)
    {
        var user = await _context.Users.Include(u => u.Photo).FirstOrDefaultAsync(u => u.Id == userId);
        return user!;
    }

    public Task<AppUser> GetQueried()
    {
        throw new NotImplementedException();
    }

    public async Task IncreaseUserBalanceAsync(AppUser user, decimal amount)
    {
        user.Balance += amount;
        await _context.SaveChangesAsync();
    }
}
