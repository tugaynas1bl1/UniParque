using FluentValidation;
using UniParque.Application.DTOs;

namespace UniParque.Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Name is required!")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required!")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required!")
            .EmailAddress().WithMessage("Email is not valid!");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required!")
            .MinimumLength(8).WithMessage("Password must contain at least 8 characters")
            .Password();

        RuleFor(x => x.ConfirmedPassword)
            .NotEmpty().WithMessage("Confirmed password is required!")
            .Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}

public class LoginValidator : AbstractValidator<LoginRequestDto>
{
    public LoginValidator()
    {

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required!")
            .EmailAddress().WithMessage("Email is not valid!");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required!")
            .MinimumLength(8).WithMessage("Password must contain at least 8 characters");
    }
}

public class CreateUserValidator : AbstractValidator<CreateUserRequestDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Name is required!")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required!")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required!")
            .EmailAddress().WithMessage("Email is not valid!");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required!")
            .MinimumLength(8).WithMessage("Password must contain at least 8 characters")
            .Password();
    }
}

public class ChangeForgottenPasswordValidator : AbstractValidator<ChangeForgottenPasswordRequestDto>
{
    public ChangeForgottenPasswordValidator()
    {

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required!");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required!")
            .MinimumLength(8).WithMessage("Password must contain at least 8 characters")
            .Password();

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Confirmed password is required!")
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
    }
}

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordValidator()
    {

        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Old password is required!")
            .MinimumLength(8).WithMessage("Password must contain at least 8 characters");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required!")
            .MinimumLength(8).WithMessage("Password must contain at least 8 characters")
            .Password();

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Confirmed password is required!")
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
    }
}

public class EditProfileValidator : AbstractValidator<EditProfileRequestDto>
{
    public EditProfileValidator()
    {

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Name is required!")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required!")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters.");
    }
}

public class EditProfileByUserIdValidator : AbstractValidator<UpdateUserRequestDto>
{
    public EditProfileByUserIdValidator()
    {

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Name is required!")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required!")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters.");
    }
}

public class RefreshValidator : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshValidator()
    {

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh Token is required!");
    }
}

public class BalanceValidator : AbstractValidator<AdjustBalanceRequestDto>
{
    public BalanceValidator()
    {

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount is required!")
            .GreaterThan(0);

        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Card number is required!")
            .Length(16).WithMessage("card number must only contain 16 characters")
            .Matches(@"^\d{16}$").WithMessage("Card number must only contain digits");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required!")
            .Must(type => type == "deposit" || type == "withdraw")
            .WithMessage("Type must be either 'deposit' or 'withdraw'");
    }
}

