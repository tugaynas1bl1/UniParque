using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using UniParque.Application.Common;
using UniParque.Application.Config;
using UniParque.Application.DTOs;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;

namespace UniParque.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAppUserRepository _userRepository;
    private readonly IUserVerificationRepository _userVerificationRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPhotoRepository _photoRepository;
    private readonly JwtConfig _jwtConfig;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    private const string RefreshTokenType = "refresh";

    private ClaimsPrincipal? User => _contextAccessor.HttpContext?.User;
    private string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);
    private bool IsAdmin => User?.IsInRole("Admin") ?? false;

    public AuthService(UserManager<AppUser> userManager, IOptions<JwtConfig> jwtConfig, IHttpContextAccessor contextAccessor, IEmailService emailService, IRefreshTokenRepository refreshTokenRepository, IAppUserRepository userRepository, IUserVerificationRepository userVerificationRepository, IPaymentRepository paymentRepository, IMapper mapper, IPhotoRepository photoRepository)
    {
        _userManager = userManager;
        _jwtConfig = jwtConfig.Value;
        _contextAccessor = contextAccessor;
        _emailService = emailService;
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _userVerificationRepository = userVerificationRepository;
        _paymentRepository = paymentRepository;
        _mapper = mapper;
        _photoRepository = photoRepository;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid email or password");

        var isValidPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

        if (!isValidPassword)
            throw new UnauthorizedAccessException("Invalid email or password");

        return await GenerateTokenAsync(user);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);

        if (existingUser is not null)
            throw new InvalidOperationException("User with this email already exists");

        var user = new AppUser
        {
            UserName = registerRequest.Email,
            Email = registerRequest.Email,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
        var result = await _userManager.CreateAsync(user, registerRequest.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(",", result.Errors.Select(e => e.Description));

            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        await _userManager.AddToRoleAsync(user, "User");

        return await GenerateTokenAsync(user);
    }

    public async Task<bool> IsUserWithEmailExists(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return false;

        return true;
    }

    private async Task<AuthResponseDto> GenerateTokenAsync(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var (refreshToken, refreshJwt) = await CreateRefreshTokenJwtAsync(user.Id, _jwtConfig.RefreshTokenExpirationDays);

        return new AuthResponseDto
        {
            AccessToken = tokenString,
            RefreshToken = refreshJwt,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            Email = user.Email!,
            IsAdmin = isAdmin
        };
    }

    private async Task<(RefreshToken refreshToken, string refreshJwt)> CreateRefreshTokenJwtAsync(string userId, int expirationDays)
    {
        var jti = Guid.NewGuid().ToString("N");
        var expiresAt = DateTime.UtcNow.AddDays(expirationDays);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.RefreshTokenSecretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim("token_type", RefreshTokenType)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = new RefreshToken
        {
            JwtId = jti,
            UserId = userId,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
        };

        await _refreshTokenRepository.AddAsync(refreshToken);

        return (refreshToken, jwtString);
    }

    private (ClaimsPrincipal principal, string jti) ValidateRefreshJwtAndGetJti(string refreshToken, bool validateLifeTime = true)
    {
        var handler = new JwtSecurityTokenHandler();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.RefreshTokenSecretKey!));

        var principal = handler.ValidateToken(refreshToken, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = validateLifeTime,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtConfig.Issuer,
            ValidAudience = _jwtConfig.Audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwt)
            throw new UnauthorizedAccessException("invalid refresh token");

        var tokenType = jwt.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;

        if (tokenType != RefreshTokenType)
            throw new UnauthorizedAccessException("invalid refresh token");

        var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        return (principal, jti);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest)
    {
        var (principal, jti) = ValidateRefreshJwtAndGetJti(refreshTokenRequest.RefreshToken);

        var storedToken = await _refreshTokenRepository.GetByJti(jti);
        if (storedToken is null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if (!storedToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token has been revoked or expired");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(userId!);

        if (user is null)
            throw new UnauthorizedAccessException("User not found");

        storedToken.RevokedAt = DateTime.UtcNow;

        var newTokens = await GenerateTokenAsync(user);

        var newStoredToken = await _refreshTokenRepository
                                    .GetByJti(GetJtiFromRefreshToken(newTokens.RefreshToken));

        if (newStoredToken is null)
            storedToken.ReplaceByJwtId = newStoredToken!.JwtId;

        await _refreshTokenRepository.SaveChangesAsync();
        return newTokens;
    }

    private static string GetJtiFromRefreshToken(string refreshJwt)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(refreshJwt)) return string.Empty;

        var jwt = handler.ReadJwtToken(refreshJwt);

        return jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
    }

    public async Task RevokeRefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest)
    {
        string? jti;
        try
        {
            (_, jti) = ValidateRefreshJwtAndGetJti(refreshTokenRequest.RefreshToken, validateLifeTime: false);
        }
        catch
        {

            return;
        }

        var storedToken = await _refreshTokenRepository.GetByJti(jti);

        if (storedToken is null || !storedToken.IsActive) return;

        storedToken.RevokedAt = DateTime.UtcNow;

        await _refreshTokenRepository.SaveChangesAsync();
    }

    public async Task SendVerificationCodeAsync(SendVerificationRequestDto request)
    {
        Random random = new Random();

        var user = await _userManager.FindByEmailAsync(request.Email);

        var userId = user?.Id;

        if (user is null)
            throw new Exception("User not found");

        var codeUserVerification = await _userVerificationRepository.GetByUserId(userId!);

        if (codeUserVerification is not null)
        {
            var code = random.Next(100000, 1000000);
            await _userVerificationRepository.ChangeCodeAsync(codeUserVerification, code);
        }
        else
        {
            var codeVerification = new UserVerificationCode
            {
                Code = random.Next(100000, 1000000),
                UserId = userId!,
                User = user,
                ExpiresAt = DateTime.UtcNow.AddMinutes(3)
            };

            user.CodeVerification = codeVerification;
            await _userVerificationRepository.AddAsync(codeVerification!);
        }

        _emailService?.SendEmailAsync(user.Email!, user.CodeVerification.Code);
    }

    public async Task<bool> ChangeForgottenPasswordAsync(ChangeForgottenPasswordRequestDto changePasswordRequest)
    {
        var user = await _userManager.FindByEmailAsync(changePasswordRequest.Email);

        var verification = await _userVerificationRepository.GetByUserId(user!.Id);

        if (user is null)
            return false;

        if (verification is null || verification.Code == default)
            return false;

        if (DateTimeOffset.UtcNow >= verification.ExpiresAt)
        {
            await _userVerificationRepository.RemoveAsync(verification);
            return false;
        }

        if (changePasswordRequest.Code != verification.Code)
            return false;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(
            user,
            token,
            changePasswordRequest.NewPassword
        );        

        if (!result.Succeeded)
        {
            var errors = string.Join(",", result.Errors.Select(x => x.Description));
            throw new Exception(errors);
        }
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _userVerificationRepository.RemoveAsync(verification);

        var refreshTokensByUser = await _refreshTokenRepository.GetByUserId(user.Id);

        await _refreshTokenRepository.RemoveRangeAsync(refreshTokensByUser);

        return true;
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequestDto changePasswordRequest)
    {
        if (string.IsNullOrEmpty(UserId))
            return false;

        var user = await _userManager
            .Users
            .Include(u => u.CodeVerification)
            .FirstOrDefaultAsync(u => u.Id == UserId);

        if (user is null)
            return false;

        var result = await _userManager.ChangePasswordAsync(
            user,
            changePasswordRequest.OldPassword, 
            changePasswordRequest.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(",", result.Errors.Select(x => x.Description));
            throw new Exception(errors);
        }

        user.UpdatedAt = DateTimeOffset.UtcNow;

        var refreshTokensByUser = await _refreshTokenRepository.GetByUserId(UserId);

        await _refreshTokenRepository.RemoveRangeAsync(refreshTokensByUser);

        return true;
    }

    public async Task<int> GetAllUsersCountAsync()
    {
        var users = _userManager.Users.ToList();

        int count = 0;

        foreach (var user in users)
        {
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                count++;
            }
        }

        return count;
    }

    public async Task<bool> AdjustBalanceAsync(AdjustBalanceRequestDto balanceRequest)
    {
        var user = await _userRepository.FindAsync(UserId!);
        var payment = new Payment();

        if (user is null)
            throw new UnauthorizedAccessException("User not found");

        if (balanceRequest.Amount < 1)
            throw new InvalidOperationException("Amount should be greater than or equal to 1");

        if (balanceRequest.Type == "deposit")
        {
            user.Balance += balanceRequest.Amount;
            payment.Status = PaymentStatus.Paid;
        }

        else if (balanceRequest.Type == "withdraw")
        {
            if (user.Balance < balanceRequest.Amount)
                throw new Exception("Insufficient balance");
            user.Balance -= balanceRequest.Amount;
            payment.Status = PaymentStatus.Withdraw;
        }

        else
            return false;

        payment.CardNumber = balanceRequest.CardNumber;
        payment.Amount = balanceRequest.Amount;
        payment.PaymentDate = DateTimeOffset.UtcNow;
        payment.UserId = user.Id;
        payment.User = user;

        user.Payments.Add(payment);

        await _paymentRepository.AddAsync(payment);

        return true;
    }

    public async Task<bool> AdjustBalanceOfAnyUserAsync(string userId, AdjustBalanceRequestDto balanceRequest)
    {
        var user = await _userRepository.FindAsync(userId!);

        if (user is null)
            throw new UnauthorizedAccessException("User not found");

        if (balanceRequest.Amount < 1)
            throw new InvalidOperationException("Amount should be greater than or equal to 1");

        if (balanceRequest.Type == "deposit")
            await _userRepository.IncreaseUserBalanceAsync(user, balanceRequest.Amount);

        else if (balanceRequest.Type == "withdraw")
        {
            if (user.Balance < balanceRequest.Amount)
                throw new Exception("Insufficient balance");
            await _userRepository.IncreaseUserBalanceAsync(user, -balanceRequest.Amount);

        }

        else
            return false;

        return true;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.Include(u => u.Photo).ToListAsync();

        if (users is null || !users.Any())
            return null!;

        var userDtos = new List<UserResponseDto>();

        foreach (var user in users)
        {
            var dto = _mapper.Map<UserResponseDto>(user);

            var roles = await _userManager.GetRolesAsync(user);
            dto.Role = roles.FirstOrDefault() ?? "User";

            userDtos.Add(dto);
        }


        return userDtos;
    }

    public async Task<UserResponseDto> GetUserByIdAsync(string id)
    {
        var user = await _userRepository.FindAsync(id);

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task <bool> SendEmailAsync(EmailMessageRequestDto message)
    {
        await _emailService.SendEmailMessageAsync(message.Email, message.Name, message.Message);
        return true;
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto createUserRequest)
    {
        var existsUser = await _userManager.FindByEmailAsync(createUserRequest.Email!);

        if (existsUser is not null)
            throw new Exception("User with this email already exists");

        var user = new AppUser
        {
            UserName = createUserRequest.Email,
            Email = createUserRequest.Email,
            FirstName = createUserRequest.FirstName,
            LastName = createUserRequest.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        var result = await _userManager.CreateAsync(user, createUserRequest.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(",", result.Errors.Select(e => e.Description));

            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        await _userManager.AddToRoleAsync(user, "User");

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<bool> DeleteUserByIdAsync(string userId)
    {
        var user = await _userRepository.FindAsync(userId);

        if (user is null)
            throw new NullReferenceException($"User with ID {userId} not found!");

       var delete = await _userManager.DeleteAsync(user);

        if (!delete.Succeeded)
            return false;

       return true;
    }

    public async Task<bool> DeleteYourAccountAsync()
    {
        var user = await _userRepository.FindAsync(UserId!);

        var delete = await _userManager.DeleteAsync(user);

        if (!delete.Succeeded)
            return false;

        return true;
    }

    public async Task<UserResponseDto> UpdateUserByIdAsync(UpdateUserRequestDto updateUserRequest)
    {
        var user = await _userRepository.FindAsync(updateUserRequest.Id);

        user.FirstName = updateUserRequest.FirstName;
        user.LastName = updateUserRequest.LastName;
        user.UpdatedAt = DateTime.Now;

        await _userManager.UpdateAsync(user);

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto> EditProfileAsync(EditProfileRequestDto editProfileRequest)
    {
        var user = await _userRepository.FindAsync(UserId!);

        if (user is null)
            throw new UnauthorizedAccessException("this user is unathorized");

        var edittedProfile = await _userRepository.ChangeProfileDataAsync(user, editProfileRequest);

        return _mapper.Map<UserResponseDto>(edittedProfile);
    }

    public Task<PagedResult<UserResponseDto>> GetPagedAsync(ParkingBranchQueryParams queryParams)
    {
        queryParams.Validate();

        var query = _userManager;
        throw new Exception();
    }

    public async Task<PhotoResponseDto> GetUserPhotoAsync()
    {
        var photo = await _photoRepository.GetByUserAsync(UserId!);

        return new PhotoResponseDto
        {
            Id = photo.Id,
            Url = photo.Url,
            PublicId = photo.PublicId,
            User = photo.User.Email!
        };
    }

    public async Task<UserResponseDto> GetCurrentUserInfoAsync()
    {
        var user = await _userRepository.FindAsync(UserId!);

        var roles = await _userManager.GetRolesAsync(user);

        var dto = _mapper.Map<UserResponseDto>(user);

        dto.Role = roles.FirstOrDefault() ?? "User";

        return dto;
    }

    public async Task<decimal> GetUserBalanceAsync()
    {
        var user = await _userRepository.FindAsync(UserId!);

        decimal balance = user.Balance;

        return balance;
    }

    public async Task<bool> AmIAdmin() 
        => User?.IsInRole("Admin") ?? false;
}
