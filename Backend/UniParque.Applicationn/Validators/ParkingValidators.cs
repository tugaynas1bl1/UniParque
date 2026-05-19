using FluentValidation;
using UniParque.Application.DTOs;

namespace UniParque.Application.Validators;

public class CreateBranchValidator : AbstractValidator<CreateParkingBranchRequestDto>
{
    public CreateBranchValidator()
    {
        RuleFor(x => x.BranchName)
            .NotEmpty().WithMessage("Branch name is required!")
            .MinimumLength(3).WithMessage("Branch name should be at least 3 characters");
    }
}

public class UpdateBranchValidator : AbstractValidator<UpdateParkingBranchRequestDto>
{
    public UpdateBranchValidator()
    {
        RuleFor(x => x.BranchName)
            .NotEmpty().WithMessage("Branch name is required!")
            .MinimumLength(3).WithMessage("Branch name should be at least 3 characters");
    }
}

public class CreateSectionValidator : AbstractValidator<CreateParkingSectionRequestDto>
{
    public CreateSectionValidator()
    {
        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("Branch ID is required!");

        RuleFor(x => x.Section)
            .NotEmpty().WithMessage("Section name is required!")
            .Matches(@"^[A-Z]$")
            .MaximumLength(1).WithMessage("Section should only contain 1 character");
    }
}

public class UpdateSectionValidator : AbstractValidator<UpdateParkingSectionRequestDto>
{
    public UpdateSectionValidator()
    {
        RuleFor(x => x.Section)
            .NotEmpty().WithMessage("Section name is required!")
            .Matches(@"^[A-Z]$")
            .MaximumLength(1).WithMessage("Section must only contain 1 character");
    }
}

public class CreateSubSectionValidator : AbstractValidator<CreateParkingSubSectionRequestDto>
{
    public CreateSubSectionValidator()
    {
        RuleFor(x => x.SectionId)
            .NotEmpty().WithMessage("Section ID is required!");

        RuleFor(x => x.SubSection)
            .NotEmpty().WithMessage("Subsection name is required!")
            .Matches(@"^[A-Z]\d{1,2}$")
            .Length(2, 3).WithMessage("Subsection must contain at least 2 characters, up to 3 characters");
    }
}

public class UpdateSubSectionValidator : AbstractValidator<UpdateParkingSubSectionRequestDto>
{
    public UpdateSubSectionValidator()
    {
        RuleFor(x => x.SubSection)
            .NotEmpty().WithMessage("Section name is required!")
            .Matches(@"^[A-Z]\d{1,2}$")
            .Length(2, 3).WithMessage("Subsection must contain at least 2 characters, up to 3 characters");
    }
}

public class CreatePlaceValidator : AbstractValidator<CreateParkingPlaceRequestDto>
{
    public CreatePlaceValidator()
    {
        RuleFor(x => x.SubSectionId)
            .NotEmpty().WithMessage("Subsection ID is required!");

        RuleFor(x => x.PlaceName)
            .NotEmpty().WithMessage("Place name is required!")
            .MinimumLength(2).WithMessage("Place must contain at least 2 characters");
    }
}

public class UpdatePlaceValidator : AbstractValidator<UpdateParkingPlaceRequestDto>
{
    public UpdatePlaceValidator()
    {
        RuleFor(x => x.PlaceName)
            .NotEmpty().WithMessage("Place name is required!")
            .MinimumLength(2).WithMessage("Place must contain at least 2 characters");
    }
}

public class CreateReservationValidator : AbstractValidator<CreateReservationRequestDto>
{
    public CreateReservationValidator()
    {
        RuleFor(x => x.PlaceId)
            .NotEmpty().WithMessage("Place ID is required!");

        RuleFor(x => x.EstimatedArrivalTime)
            .NotEmpty().WithMessage("Estimated arrival time is required!");

        RuleFor(x => x.CarNumber)
            .NotEmpty().WithMessage("Car number is required!")
            .MinimumLength(7).WithMessage("Car number must be at least 7 characters");
    }
}

public class UpdateReservationValidator : AbstractValidator<UpdateReservationRequestDto>
{
    public UpdateReservationValidator()
    {
        RuleFor(x => x.PlaceId)
            .NotEmpty().WithMessage("Place ID is required!");

        RuleFor(x => x.CarNumber)
            .NotEmpty().WithMessage("Car number is required!")
            .MinimumLength(7).WithMessage("Car number must be at least 7 characters");
    }
}

public class CreateReservationWithCardValidator : AbstractValidator<CreateReservationWithCardRequestDto>
{
    public CreateReservationWithCardValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Card number is required!")
            .Length(16).WithMessage("card number must only contain 16 characters")
            .Matches(@"^\d{16}$").WithMessage("Card number must only contain digits");

        RuleFor(x => x.Reservation.PlaceId)
            .NotEmpty().WithMessage("Place ID is required!");

        RuleFor(x => x.Reservation.EstimatedArrivalTime)
            .NotEmpty().WithMessage("Estimated arrival time is required!");

        RuleFor(x => x.Reservation.CarNumber)
            .NotEmpty().WithMessage("Car number is required!")
            .MinimumLength(7).WithMessage("Car number must be at least 7 characters");
    }
}

public class CreateReservationWithSpecificUserValidator : AbstractValidator<CreateReservationRequestWithSpecificUserDto>
{
    public CreateReservationWithSpecificUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Place ID is required!");

        RuleFor(x => x.PlaceId)
            .NotEmpty().WithMessage("Place ID is required!");

        RuleFor(x => x.EstimatedArrivalTime)
            .NotEmpty().WithMessage("Estimated arrival time is required!");

        RuleFor(x => x.CarNumber)
            .NotEmpty().WithMessage("Car number is required!")
            .MinimumLength(7).WithMessage("Car number must be at least 7 characters");
    }
}