using Microsoft.EntityFrameworkCore;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;
using UniParque_Infrastructure.Persistence;

namespace UniParque_Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly UniParqueDBContext _context;

    public RefreshTokenRepository(UniParqueDBContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<RefreshToken> GetByJti(string jti)
    {
        var refreshToken = await _context
                                .RefreshTokens
                                .FirstOrDefaultAsync(rt => rt.JwtId == jti);
        return refreshToken!;
    }

    public async Task<IEnumerable<RefreshToken>> GetByUserId(string userId)
    {
        return await _context
                    .RefreshTokens
                    .Where(rt => rt.UserId == userId)
                    .ToListAsync();
    }

    public async Task RemoveRangeAsync(IEnumerable<RefreshToken> refreshTokens)
    {
        _context.RefreshTokens.RemoveRange(refreshTokens);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
