using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UniParque.Application.DTOs;
using UniParque.Application.Services;
using UniParque_Domain.Entities;

namespace UniParque.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ParkingBranch Mappings
        CreateMap<ParkingBranch, ParkingBranchResponseDto>();

        CreateMap<CreateParkingBranchRequestDto, ParkingBranch>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Sections, opt => opt.Ignore());

        CreateMap<UpdateParkingBranchRequestDto, ParkingBranch>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Sections, opt => opt.Ignore());


        // ParkingSection Mappings
        CreateMap<ParkingSection, ParkingSectionResponseDto>()
            .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch!.BranchName));

        CreateMap<CreateParkingSectionRequestDto, ParkingSection>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.SubSections, opt => opt.Ignore())
            .ForMember(dest => dest.Branch, opt => opt.Ignore());

        CreateMap<UpdateParkingSectionRequestDto, ParkingSection>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.SubSections, opt => opt.Ignore())
            .ForMember(dest => dest.Branch, opt => opt.Ignore());

        // ParkingSubSection Mappings
        CreateMap<ParkingSubSection, ParkingSubSectionResponseDto>()
            .ForMember(dest => dest.Section, opt => opt.MapFrom(src => src.Section!.Section));

        CreateMap<CreateParkingSubSectionRequestDto, ParkingSubSection>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Places, opt => opt.Ignore())
            .ForMember(dest => dest.Section, opt => opt.Ignore());

        CreateMap<UpdateParkingSubSectionRequestDto, ParkingSubSection>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Places, opt => opt.Ignore())
            .ForMember(dest => dest.Section, opt => opt.Ignore());

        // ParkingPlace Mappings
        CreateMap<ParkingPlace, ParkingPlaceResponseDto>()
            .ForMember(dest => dest.SubSection, opt => opt.MapFrom(src => src.SubSection!.SubSection));

        CreateMap<CreateParkingPlaceRequestDto, ParkingPlace>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.SubSection, opt => opt.Ignore());

        CreateMap<UpdateParkingPlaceRequestDto, ParkingPlace>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.SubSection, opt => opt.Ignore());

        // ParkingReservation Mappings
        CreateMap<ParkingReservation, ReservationResponseDto>()
            .ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place.PlaceName))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => $"{src.User.Email} ({src.User.FirstName} {src.User.LastName})"))
            .ForMember(dest => dest.SubSection, opt => opt.MapFrom(src => src.Place.SubSection!.SubSection))
            .ForMember(dest => dest.Section, opt => opt.MapFrom(src => src.Place.SubSection!.Section!.Section))
            .ForMember(dest => dest.IsPlaceOccupied, opt => opt.MapFrom(src => src.Place.IsTaken))
            .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Place.SubSection!.Section!.Branch!.BranchName))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.TotalPrice));

        CreateMap<CreateReservationRequestWithSpecificUserDto, ParkingReservation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Place, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ReservationStatus.Active));

        CreateMap<CreateReservationRequestDto, ParkingReservation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Place, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ReservationStatus.Active));

        CreateMap<CreateReservationWithCardRequestDto, ParkingReservation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Place, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CarNumber, opt => opt.MapFrom(src => src.Reservation.CarNumber))
            .ForMember(dest => dest.PlaceId, opt => opt.MapFrom(src => src.Reservation.PlaceId))
            .ForMember(dest => dest.EstimatedArrivalTime, opt => opt.MapFrom(src => src.Reservation.EstimatedArrivalTime))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ReservationStatus.Active));


        CreateMap<UpdateReservationRequestDto, ParkingReservation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Place, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        // Photo Mappings
        CreateMap<Photo, PhotoResponseDto>();

        // AppUser Mappings
        CreateMap<AppUser, UserResponseDto>()
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo!.Url));
    }
}
