using UniParque_Domain.Entities;

namespace UniParque.Application.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> AddAsync(RefreshToken refreshToken);
    Task<RefreshToken> GetByJti(string jti);
    Task<IEnumerable<RefreshToken>> GetByUserId (string userId);
    Task RemoveRangeAsync(IEnumerable<RefreshToken> refreshTokens);
    Task SaveChangesAsync();
}
