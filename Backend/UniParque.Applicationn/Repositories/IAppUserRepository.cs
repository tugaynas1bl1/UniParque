using UniParque.Application.DTOs;
using UniParque_Domain.Entities;

namespace UniParque.Application.Repositories;

public interface IAppUserRepository
{
    Task<AppUser> FindAsync(string userId);
    Task IncreaseUserBalanceAsync(AppUser user, decimal amount);
    Task<AppUser> ChangeProfileDataAsync(AppUser user, EditProfileRequestDto request);
    Task<AppUser> GetQueried();
}
