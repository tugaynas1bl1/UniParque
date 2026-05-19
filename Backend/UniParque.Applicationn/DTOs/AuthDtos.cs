namespace UniParque.Application.DTOs;

public class AuthResponseDto
{
    /// <summary>
    /// JWT access token used for authorized API requests
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Expiration date and time of the access token
    /// </summary>
    /// <example>2026-03-02T15:30:00Z</example>
    public DateTimeOffset AccessTokenExpiresAt { get; set; }

    /// <summary>
    /// Refresh token used to obtain a new access token
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXH78A...</example>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Expiration date and time of the refresh token
    /// </summary>
    /// <example>2026-03-09T15:30:00Z</example>
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }

    /// <summary>
    /// Email address of the authenticated user
    /// </summary>
    /// <example>user@example.com</example>
    public string Email { get; set; } = string.Empty;

    public bool IsAdmin { get; set; } = false;
}

public class RegisterRequestDto
{
    /// <summary>
    /// User Name
    /// </summary>
    /// <example>John</example>
    public string FirstName { get; set; } = string.Empty;
    /// <summary>
    /// User Lastname
    /// </summary>
    /// <example>Doe</example>
    public string LastName { get; set; } = string.Empty;
    /// <summary>
    /// User Email
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Password
    /// </summary>
    /// <example>P@ssword123!</example>
    public string Password { get; set; } = string.Empty;
    /// <summary>
    /// Confirmed Password
    /// </summary>
    /// <example>P@ssword123!</example>
    public string ConfirmedPassword { get; set; } = string.Empty;
}

public class LoginRequestDto
{
    /// <summary>
    /// User Email
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    /// <example>P@ssword123!</example>
    public string Password { get; set; } = string.Empty;
}

public class EditProfileRequestDto
{
    /// <summary>
    /// Firstname of the user.
    /// </summary>
    /// <example>John</example>
    public string FirstName { get; set; }

    /// <summary>
    /// Lastname of the user.
    /// </summary>
    /// <example>Doe</example>
    public string LastName { get; set; }
}

public class ChangeForgottenPasswordRequestDto
{
    /// <summary>
    /// Verification code sent to the user for password change confirmation.
    /// </summary>
    /// <example>123456</example>
    public int Code { get; set; }

    /// <summary>
    /// The email of user
    /// </summary>
    /// <example>uniparque@example.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The new password the user wants to set.
    /// </summary>
    /// <example>NewPassword123</example>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// The confirmation of new password the user wants to set.
    /// </summary>
    /// <example>NewPassword123</example>
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

public class ChangePasswordRequestDto
{
    /// <summary>
    /// The old password user has set.
    /// </summary>
    /// <example>Password123</example>
    public string OldPassword { get; set; }

    /// <summary>
    /// The new password the user wants to set.
    /// </summary>
    /// <example>NewPassword123</example>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// The confirmation of new password the user wants to set.
    /// </summary>
    /// <example>NewPassword123</example>
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

public class AdjustBalanceRequestDto
{
    /// <summary>
    /// Amount of money to be deposited or withdrawn from the user's balance
    /// </summary>
    /// <example>50.00</example>
    public decimal Amount { get; set; }

    /// <summary>
    /// Card number used for the transaction (required for deposit or withdraw operations)
    /// </summary>
    /// <example>4098584487322842</example>
    public string CardNumber { get; set; } = string.Empty;

    /// <summary>
    /// Type of balance operation (deposit/withdraw)
    /// </summary>
    /// <example>deposit</example>
    public string Type { get; set; } = string.Empty;
}

public class RefreshTokenRequestDto
{
    /// <summary>
    /// RefreshToken
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    public string RefreshToken { get; set; } = string.Empty;
}

public class UserResponseDto
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    /// <example>c8f1a9d2-5c0a-4e7c-bb6a-123456789abc</example>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User's first name
    /// </summary>
    /// <example>John</example>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    /// <example>Doe</example>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Current balance of the user
    /// </summary>
    /// <example>150.75</example>
    public decimal Balance { get; set; }

    /// <summary>
    /// Date and time when the user account was created
    /// </summary>
    /// <example>2026-03-10T12:30:00Z</example>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Role of the user
    /// </summary>
    /// <example>User</example>
    public string Role { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
}

public class CreateUserRequestDto
{
    /// <summary>
    /// User's first name
    /// </summary>
    /// <example>John</example>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    /// <example>Doe</example>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    /// <example>P@ssword123!</example>
    public string Password {  get; set; } = string.Empty;
}

public class UpdateUserRequestDto
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    /// <example>c8f1a9d2-5c0a-4e7c-bb6a-123456789abc</example>
    public string Id { get; set; } = string.Empty;
    /// <summary>
    /// User's first name
    /// </summary>
    /// <example>John</example>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    /// <example>Doe</example>
    public string LastName { get; set; } = string.Empty;
}

public class PayFromBalanceRequestDto
{
    public decimal Amount { get; set; }
}

public class SendVerificationRequestDto
{
    public string Email {  set; get; } = string.Empty;
}

public class EmailMessageRequestDto
{
    public string Name {  get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}