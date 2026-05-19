using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniParque.Application.Common;
using UniParque.Application.DTOs;
using UniParque.Application.Services;
using UniParque_Domain.Entities;
using UniParque_Infrastructure.Persistence;

namespace UniParque.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IPhotoService _photoService;
    private readonly UniParqueDBContext _context;
    private readonly IMapper _mapper;

    private string? UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier);

    public AuthController(IAuthService authService, IPhotoService photoService, IMapper mapper, UniParqueDBContext context)
    {
        _authService = authService;
        _photoService = photoService;
        _mapper = mapper;
        _context = context;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="registerRequest">User registration data</param>
    /// <returns>Access and refresh tokens wrapped in AuthResponseDto</returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register(
        [FromBody] RegisterRequestDto registerRequest)
    {
        var result = await _authService.RegisterAsync(registerRequest);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "User registered successfully"));
    }

    [HttpPost("send-email-message")]
    public async Task<ActionResult<ApiResponse<bool>>> SendEmailMessage(
        [FromBody] EmailMessageRequestDto message)
    {
        var result = await _authService.SendEmailAsync(message);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Email sent successfully"));
    }

    [HttpGet("user-exists/{email}")]
    public async Task<ActionResult<ApiResponse<bool>>> IsUserWithTheEmailExists(string email)
    {
        var result = await _authService.IsUserWithEmailExists(email);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="loginRequest">User login credentials</param>
    /// <returns>Access and refresh tokens wrapped in AuthResponseDto</returns>
    /// <response code="200">User logged in successfully</response>
    /// <response code="401">Invalid email or password</response>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(
        [FromBody] LoginRequestDto loginRequest)
    {
        var result = await _authService.LoginAsync(loginRequest);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "User logged in successfully"));
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <param name="refreshTokenRequest">Refresh token payload</param>
    /// <returns>New access and refresh tokens</returns>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="401">Invalid refresh token</response>
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh(
        [FromBody] RefreshTokenRequestDto refreshTokenRequest)
    {
        var result = await _authService.RefreshTokenAsync(refreshTokenRequest);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Token refreshed successfully"));
    }

    /// <summary>
    /// Revoke refresh token (Logout)
    /// </summary>
    /// <param name="refreshTokenRequest">Refresh token to revoke</param>
    /// <returns>Confirmation message</returns>
    /// <response code="200">Token revoked successfully</response>
    /// <response code="400">Invalid token</response>
    [HttpPost("revoke")]
    public async Task<ActionResult<ApiResponse<object>>> Revoke(
        [FromBody] RefreshTokenRequestDto refreshTokenRequest)
    {
        await _authService.RevokeRefreshTokenAsync(refreshTokenRequest);
        return Ok(ApiResponse<object>.SuccessResponse("Token revoked successfully"));
    }

    [HttpPost("add-photo")]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<ActionResult<PhotoResponseDto>> AddPhoto(IFormFile file)
    {
        try
        {
            var photoDto = await _photoService.AddPhotoAsync(file);
            return Ok(photoDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("delete-photo")]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<ActionResult> DeletePhoto()
    {
        await _photoService.DeletePhotoAsync();
        return Ok("Photo deletion went successful!");
    }

    [HttpPost("send-verification-code")]
    public async Task<ActionResult<ApiResponse<bool>>> SendVerification(SendVerificationRequestDto request)
    {
        await _authService.SendVerificationCodeAsync(request);
        return Ok();
    }

    [HttpPut("change-forgotten-password")]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeForgottenPassword(
        [FromBody] ChangeForgottenPasswordRequestDto passwordRequest)
    {
        var result = await _authService.ChangeForgottenPasswordAsync(passwordRequest);

        if (!result)
            return BadRequest("Verification code or something else is incorrect");

        return Ok(ApiResponse<object>.SuccessResponse(result, "Password changed successfully!"));
    }

    [HttpPut("change-password")]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<ActionResult<ApiResponse<bool>>> ChangePassword(
        [FromBody] ChangePasswordRequestDto passwordRequest)
    {
        var result = await _authService.ChangePasswordAsync(passwordRequest);

        if (!result)
            return BadRequest("Something went wrong while changing the password!");

        return Ok(ApiResponse<object>.SuccessResponse(result, "Password changed successfully!"));
    }

    [HttpPatch("adjust-balance")]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<ActionResult<ApiResponse<bool>>> AdjustBalance(
       AdjustBalanceRequestDto balanceRequest)
    {
        var result = await _authService.AdjustBalanceAsync(balanceRequest);

        if (!result)
            return BadRequest("Something went wrong while transaction");

        return Ok(ApiResponse<object>.SuccessResponse(result, $"Transaction went successful!"));
    }

    [HttpPatch("adjust-balance/{userId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<bool>>> AdjustBalanceOfAnyUser(
       string userId, AdjustBalanceRequestDto balanceRequest)
    {
        var result = await _authService.AdjustBalanceOfAnyUserAsync(userId, balanceRequest);

        if (!result)
            return BadRequest("Something went wrong while transaction");

        return Ok(ApiResponse<object>.SuccessResponse(result, $"Transaction went successful!"));
    }

    [HttpGet("all-users")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetAllUsers()
    {
        var users = await _authService.GetAllUsersAsync();

        if (users is null)
            return BadRequest("No any users");

        return Ok(ApiResponse<IEnumerable<UserResponseDto>>.SuccessResponse(users));
    }

    [HttpGet("user/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetUserById(string id)
    {
        var user = await _authService.GetUserByIdAsync(id);

        if (user is null)
            return BadRequest($"User with ID {id} not found!");

        return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user));
    }

    [HttpPatch("edit-profile")]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> EditProfile(
        EditProfileRequestDto editProfileRequest)
    {
        var user = await _authService.EditProfileAsync(editProfileRequest);

        if (user is null)
            return BadRequest("This user not found or unauthorized");

        return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user));
    }

    [HttpGet("users-count")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<int>>> GetUsersCount()
    {
        var count = await _authService.GetAllUsersCountAsync();
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    [HttpPatch("create-user")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> CreateUser(
        [FromBody] CreateUserRequestDto createRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdUser = await _authService.CreateUserAsync(createRequest);

        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id },
            ApiResponse<UserResponseDto>.SuccessResponse(createdUser, "User created successfully"));
    }

    [HttpDelete("delete-your-account")]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteYourAccount()
    {
        var deletedAcc = await _authService.DeleteYourAccountAsync();

        if (!deletedAcc)
            return BadRequest("Something went wrong while deleting your account");

        return Ok(ApiResponse<object>.SuccessResponse(deletedAcc, "Your account deleted successfully!"));
    }

    [HttpDelete("user/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUserById(string id)
    {
        var deletedAcc = await _authService.DeleteUserByIdAsync(id);

        if (!deletedAcc)
            return BadRequest("Something went wrong while deleting the account");

        return Ok(ApiResponse<object>.SuccessResponse(deletedAcc, "Account deleted successfully"));
    }

    [HttpPatch("update-user")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> UpdateUserById(UpdateUserRequestDto updateRequest)
    {
        var updatedAcc = await _authService.UpdateUserByIdAsync(updateRequest);

        if (updatedAcc is null)
            return BadRequest("User not found!");

        return Ok(ApiResponse<UserResponseDto>.SuccessResponse(updatedAcc, "Account updated successfully"));
    }

    [HttpGet("profile-photo")]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<ActionResult<ApiResponse<PhotoResponseDto>>> GetProfilePhoto()
    {
        var photo = await _authService.GetUserPhotoAsync();

        return Ok(ApiResponse<PhotoResponseDto>.SuccessResponse(photo));
    }

    [HttpGet("my-info")]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetCurrentUserInfo()
    {
        var user = await _authService.GetCurrentUserInfoAsync();

        if (user is null)
            return BadRequest("User not found");

        return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user));
    }

    [HttpGet("my-balance")]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<ActionResult<ApiResponse<decimal>>> GetUserBalance()
    {
        var balance = await _authService.GetUserBalanceAsync();
        return Ok(ApiResponse<decimal>.SuccessResponse(balance));
    }

    [HttpGet("am-i-admin")]
    public async Task<ActionResult<ApiResponse<bool>>> GetAmIAdmin()
    {
        var isAdmin = await _authService.AmIAdmin();
        return Ok(ApiResponse<bool>.SuccessResponse(isAdmin));
    }
}
