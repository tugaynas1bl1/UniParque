using UniParque_Domain.Entities;

namespace UniParque.Application.Repositories;

public interface IUserVerificationRepository
{
    Task<UserVerificationCode> AddAsync(UserVerificationCode verificationCode);
    Task<UserVerificationCode> GetByUserId(string userId);
    Task RemoveAsync(UserVerificationCode verificationCode);
    Task ChangeCodeAsync(UserVerificationCode verificationCode, int code);
}
