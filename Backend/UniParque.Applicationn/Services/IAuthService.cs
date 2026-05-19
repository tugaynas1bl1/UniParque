using UniParque.Application.Common;
using UniParque.Application.DTOs;
using UniParque_Domain.Entities;

namespace UniParque.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
    Task<bool> IsUserWithEmailExists(string email);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
    Task<AuthResponseDto> RefreshTokenAsync (RefreshTokenRequestDto refreshTokenRequest);
    Task<UserResponseDto> EditProfileAsync(EditProfileRequestDto editProfileRequest);
    Task RevokeRefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest);
    Task SendVerificationCodeAsync(SendVerificationRequestDto request);
    Task<bool> ChangeForgottenPasswordAsync(ChangeForgottenPasswordRequestDto changePasswordRequest);
    Task<bool> SendEmailAsync(EmailMessageRequestDto message);
    Task<bool> ChangePasswordAsync(ChangePasswordRequestDto changePasswordRequest);
    Task<bool> AdjustBalanceAsync(AdjustBalanceRequestDto balanceRequestDto);
    Task<bool> AdjustBalanceOfAnyUserAsync(string userId, AdjustBalanceRequestDto balanceRequestDto);
    Task<decimal> GetUserBalanceAsync();
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto> GetUserByIdAsync(string id);
    Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto createUserRequest);
    Task<bool> DeleteUserByIdAsync(string userId);
    Task<bool> DeleteYourAccountAsync();
    Task<UserResponseDto> UpdateUserByIdAsync(UpdateUserRequestDto updateUserRequest);
    Task<PagedResult<UserResponseDto>> GetPagedAsync(ParkingBranchQueryParams queryParams);
    Task<PhotoResponseDto> GetUserPhotoAsync();
    Task<UserResponseDto> GetCurrentUserInfoAsync();
    Task<bool> AmIAdmin();
    Task<int> GetAllUsersCountAsync();
}
