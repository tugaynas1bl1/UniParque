using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniParque.Application.Common;
using UniParque.Application.DTOs;
using UniParque.Application.Hubs;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;

namespace UniParque.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IMapper _mapper;
    private readonly IReservationRepository _reservationRepository;
    private readonly IParkingPlaceRepository _parkingPlaceRepository;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IAppUserRepository _userRepository;
    private readonly IPaymentService _paymentService;
    private readonly IHubContext<ReservationHub> _hub;  

    private const int priceHourly = 10;

    public ReservationService(IMapper mapper, IReservationRepository reservationRepository, IHttpContextAccessor contextAccessor, IParkingPlaceRepository parkingPlaceRepository, IPaymentService paymentService, IAppUserRepository userRepository, IHubContext<ReservationHub> hub)
    {
        _mapper = mapper;
        _reservationRepository = reservationRepository;
        _contextAccessor = contextAccessor;
        _parkingPlaceRepository = parkingPlaceRepository;
        _paymentService = paymentService;
        _userRepository = userRepository;
        _hub = hub;
    }

    private ClaimsPrincipal? User => _contextAccessor.HttpContext?.User;
    private string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);
    private bool IsAdmin => User?.IsInRole("Admin") ?? false;

    private async Task<ParkingPlace> ValidateReservationAsync(CreateReservationRequestDto request)
    {
        var parkingPlace = await _parkingPlaceRepository.GetByIdWithSubSectionAsync(request.PlaceId)
            ?? throw new InvalidOperationException("This reservation place doesn't exist");

        if (parkingPlace.IsReserved)
            throw new InvalidOperationException("This place has already been reserved!");

        if (parkingPlace.IsTaken)
            throw new InvalidOperationException("This place is taken right now!");

        var reservedByCarNumb = await _reservationRepository.GetByCarNumber(request.CarNumber);

        if (reservedByCarNumb is not null)
            throw new InvalidOperationException("There is already a reservation for this car");

        if (request.EstimatedArrivalTime <= DateTimeOffset.UtcNow)
            throw new InvalidTimeZoneException("Arrival time cannot be earlier than now!");

        return parkingPlace;
    }

    public async Task<decimal> CalculatePriceAsync(CalculatePriceRequestDto calculateRequest)
    {
        var period = calculateRequest.EstimatedArrivalTime - DateTimeOffset.UtcNow;

        var hours = (decimal)Math.Ceiling(period.TotalHours);

        var totalPrice = priceHourly * hours;

        return totalPrice;
    }

    public async Task<ReservationResponseDto> CreateAsync(CreateReservationWithCardRequestDto createdReservationRequest)
    {
        var parkingPlace = await ValidateReservationAsync(createdReservationRequest.Reservation);

        CalculatePriceRequestDto calculateRequest = new CalculatePriceRequestDto
        {
            EstimatedArrivalTime = createdReservationRequest.Reservation.EstimatedArrivalTime
        };

        var totalPrice = await CalculatePriceAsync(calculateRequest);

        var createdReservation = _mapper.Map<ParkingReservation>(createdReservationRequest);

        createdReservation.EstimatedArrivalTime = createdReservation.EstimatedArrivalTime!.Value.AddMinutes(20);

        createdReservation.UserId = UserId!;
        createdReservation.TotalPrice = totalPrice;
        createdReservation.CreatedAt = DateTime.UtcNow;
        createdReservation.Status = ReservationStatus.Active;


        parkingPlace!.IsReserved = true;

        var paymentResult = await _paymentService.ProcessPaymentAsync(new PaymentRequest
        {
            CardNumber = createdReservationRequest.CardNumber,
            Amount = totalPrice,
            UserId = UserId!
        });

        if (!paymentResult.IsSuccess)
            throw new Exception("Payment failed!");
        
        await _parkingPlaceRepository.UpdateAsync(parkingPlace);

        await _reservationRepository.AddAsync(createdReservation);

        await _hub.Clients.Group($"branch_{parkingPlace!.SubSection!.Section.BranchId}")
    .SendAsync("PlaceReserved", new
    {
        placeId = parkingPlace.Id,
        isReserved = true
    });

        return _mapper.Map<ReservationResponseDto>(createdReservation);
    }

    public async Task<ReservationResponseDto> CreateByBalanceAsync(CreateReservationRequestDto createdReservationRequest)
    {
        var parkingPlace = await ValidateReservationAsync(createdReservationRequest);

        CalculatePriceRequestDto calculateRequest = new CalculatePriceRequestDto
        {
            EstimatedArrivalTime = createdReservationRequest.EstimatedArrivalTime
        };
        var totalPrice = await CalculatePriceAsync(calculateRequest);

        var createdReservation = _mapper.Map<ParkingReservation>(createdReservationRequest);

        createdReservation.EstimatedArrivalTime = createdReservation.EstimatedArrivalTime!.Value.AddMinutes(20);

        createdReservation.UserId = UserId!;
        createdReservation.TotalPrice = totalPrice;
        createdReservation.CreatedAt = DateTime.UtcNow;
        createdReservation.Status = ReservationStatus.Active;

        parkingPlace!.IsReserved = true;

        var user = await _userRepository.FindAsync(UserId!);

        if (totalPrice > user.Balance)
            throw new Exception("Insufficient balance!");
        
        user.Balance -= totalPrice;

        await _parkingPlaceRepository.UpdateAsync(parkingPlace);

        await _reservationRepository.AddAsync(createdReservation);

        await _hub.Clients.Group($"branch_{parkingPlace!.SubSection!.Section.BranchId}")
    .SendAsync("PlaceReserved", new
    {
        placeId = parkingPlace.Id,
        isReserved = true
    });

        return _mapper.Map<ReservationResponseDto>(createdReservation);
    }

    public async Task<ReservationResponseDto> CreateWithSpecificUserAsync(CreateReservationRequestWithSpecificUserDto request)
    {
        var parkingPlace = await _parkingPlaceRepository.GetByIdWithSubSectionAsync(request.PlaceId)
            ?? throw new InvalidOperationException("This reservation place doesn't exist");

        var reservation = await _reservationRepository.GetByPlaceIdAsync(parkingPlace.Id);

        if (reservation is not null)
            throw new InvalidOperationException("This place has already been reserved!");

        var reservedByCarNumb = await _reservationRepository.GetByCarNumber(request.CarNumber);

        if (reservedByCarNumb is not null)
            throw new InvalidOperationException("There is already a reservation for this car");

        if (request.EstimatedArrivalTime <= DateTimeOffset.UtcNow)
            throw new InvalidTimeZoneException("Arrival time cannot be earlier than now!");

        var totalPrice = await CalculatePriceAsync(new CalculatePriceRequestDto
        {
            EstimatedArrivalTime = request.EstimatedArrivalTime
        });

        var createdReservation = _mapper.Map<ParkingReservation>(request);

        createdReservation.TotalPrice = totalPrice;
        createdReservation.CreatedAt = DateTime.UtcNow;

        parkingPlace.IsReserved = true;
        await _reservationRepository.SetStatus(createdReservation, ReservationStatus.Active);

        await _parkingPlaceRepository.UpdateAsync(parkingPlace);

        await _reservationRepository.AddAsync(createdReservation);

        return _mapper.Map<ReservationResponseDto>(createdReservation);
    }

    public async Task<bool> DeleteAllAsync()
    {
        var allReservations = await _reservationRepository.GetAllAsync();

        if (allReservations is null || !allReservations.Any())
            return false;

        var selectedReservations = IsAdmin
            ? allReservations 
            : allReservations.Where(r => r!.UserId == UserId)?.ToList();

        if (selectedReservations is null || !selectedReservations.Any())
            throw new UnauthorizedAccessException("Not authorized to delete all reservations");

        foreach (var reservation in selectedReservations)
        {
            await _reservationRepository.RemoveAsync(reservation!);

            var place = await _parkingPlaceRepository.GetByIdWithSubSectionAsync(reservation!.PlaceId);
            place!.IsReserved = false;

            await _parkingPlaceRepository.UpdateAsync(place);
        }

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var reservation = await _reservationRepository.FindAsync(id);

        if (reservation is null)
            return false;

        if (reservation.UserId == UserId || IsAdmin)
        {
            await _reservationRepository.RemoveAsync(reservation);

            var place = await _parkingPlaceRepository.GetByIdWithSubSectionAsync(reservation!.PlaceId);
            place!.IsReserved = false;
            place!.IsTaken = false;

            await _parkingPlaceRepository.UpdateAsync(place);
        }
        else
            throw new UnauthorizedAccessException("Not authorized to delete that reservation");

        return true;
    }

    public async Task<IEnumerable<ReservationResponseDto>> GetAllAsync()
    {
        var allReservations = await _reservationRepository.GetAllAsync();

        var selectedReservations = IsAdmin
            ? allReservations.ToList()
            : allReservations.Where(r => r!.UserId == UserId)?.ToList();

        var mappedReservations = allReservations.Select(r =>
        {
            var dto = _mapper.Map<ReservationResponseDto>(r);
            dto.isReservedByMe = r!.UserId == UserId;
            return dto;
        });


        return mappedReservations.Any() ? mappedReservations! : null!;
    }

    public async Task<ReservationResponseDto> GetByIdAsync(Guid id)
    {
        var reservation = await _reservationRepository.GetByIdWithUsersAndPlacesAsync(id);

        return reservation is not null && (reservation.UserId == UserId || IsAdmin) ? _mapper.Map<ReservationResponseDto>(reservation) : null!; 
    }

    public async Task<IEnumerable<ReservationResponseDto>> GetByUserIdAsync(string userId)
    {
        var reservationsByUser = await _reservationRepository.GetByUserIdAsync(userId);

        if (reservationsByUser is null || !reservationsByUser.Any())
            return null!;

        if (IsAdmin)
            return _mapper.Map<IEnumerable<ReservationResponseDto>>(reservationsByUser);
        else
            throw new UnauthorizedAccessException("Not authorized to get reservations by users");
    }

    public async Task<ReservationResponseDto> UpdateAsync(Guid id, UpdateReservationRequestDto updatedReservationRequest)
    {        
        var updatedReservation = await _reservationRepository.GetByIdWithUsersAndPlacesAsync(id);

        if (updatedReservation is null)
            throw new NullReferenceException();

        if (updatedReservation.UserId != UserId || !IsAdmin)
            throw new UnauthorizedAccessException("Unauthorized to update this reservation");

        var place = await _parkingPlaceRepository.GetByIdWithSubSectionAsync(updatedReservation!.PlaceId);
        place!.IsReserved = false;

        await _parkingPlaceRepository.UpdateAsync(place);

        _mapper.Map(updatedReservationRequest, updatedReservation);

        await _reservationRepository.UpdateAsync(updatedReservation);

        place = await _parkingPlaceRepository.GetByIdWithSubSectionAsync(updatedReservation!.PlaceId);
        place!.IsReserved = true;

        await _parkingPlaceRepository.UpdateAsync(place);

        return _mapper.Map<ReservationResponseDto>(updatedReservation);
    }

    public async Task<bool> ConfirmArrivalAsync(Guid reservationId)
    {
        var reservation = await _reservationRepository.GetByIdWithUsersAndPlacesAsync(reservationId);
        if (reservation is null)
            throw new NullReferenceException("Reservation not found");
        var currentUser = await _userRepository.FindAsync(UserId!);

        if (reservation!.UserId != UserId && !IsAdmin)
            throw new UnauthorizedAccessException("Not your reservation!");

        if (reservation.Place.IsTaken)
            throw new UnauthorizedAccessException("Already confirmed!");

        reservation.Place.IsTaken = true;
        reservation.Place.IsReserved = false;

        reservation.EstimatedArrivalTime = reservation.EstimatedArrivalTime!.Value.AddMinutes(-20);
        await _reservationRepository.SetStatus(reservation, ReservationStatus.CheckedIn);

        var restTimeSpan = DateTimeOffset.UtcNow - reservation.CreatedAt;

        var hours = (decimal)Math.Ceiling(restTimeSpan.TotalHours);

        var totalPrice = priceHourly * hours;

        var amount = reservation.TotalPrice - totalPrice;

        if (amount >= 0)
            currentUser!.Balance += amount;

        reservation.EstimatedArrivalTime = null;

        await _parkingPlaceRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> EndReservationSessionAndLeaveAsync(Guid reservationId)
    {
        var reservation = await _reservationRepository.GetByIdWithUsersAndPlacesAsync(reservationId);

        if (reservation is null)
            throw new NullReferenceException("Reservation not found");

        if (reservation.UserId != UserId && !IsAdmin)
            throw new UnauthorizedAccessException("This reservation belongs to another user");

        if (!reservation.Place.IsTaken)
            throw new UnauthorizedAccessException("You haven't arrived yet!");

        reservation.Place.IsTaken = false;
        reservation.ReservationDeletionTime = DateTimeOffset.UtcNow.AddYears(1);

        reservation.EstimatedArrivalTime = null;
        await _reservationRepository.SetStatus(reservation, ReservationStatus.Completed);

        return true;
    }

    public async Task<bool> CancelReservationAsync(Guid reservationId)
    {
        var reservation = await _reservationRepository.GetByIdWithUsersAndPlacesAsync(reservationId);
        if (reservation is null)
            throw new NullReferenceException("Reservation not found");
        var currentUser = await _userRepository.FindAsync(UserId!);

        if (reservation!.UserId != UserId)
            throw new UnauthorizedAccessException("Not your reservation!");

        if (reservation.Place.IsTaken)
            throw new UnauthorizedAccessException("You have already taken the reservation");

        reservation.Place.IsReserved = false;
        reservation.ReservationDeletionTime = DateTimeOffset.UtcNow.AddYears(1);

        reservation.EstimatedArrivalTime = reservation.EstimatedArrivalTime!.Value.AddMinutes(-20);

        var restTimeSpan = DateTimeOffset.UtcNow - reservation.CreatedAt;

        var hours = (decimal)Math.Ceiling(restTimeSpan.TotalHours);

        var totalPrice = priceHourly * hours;

        var amount = reservation.TotalPrice - totalPrice;

        if (amount >= 0)
            currentUser!.Balance += amount;

        reservation.EstimatedArrivalTime = null;

        await _reservationRepository.SetStatus(reservation, ReservationStatus.Cancelled);
        await _hub.Clients.Group($"branch_{reservation.Place!.SubSection!.Section.BranchId}")
    .SendAsync("PlaceEmpty", new
    {
        placeId = reservation.Place.Id,
        isReserved = false,
        isOccupied = false,
    });

        return true;
    }

    public async Task<IEnumerable<ReservationResponseDto>> GetByUserId(string userId)
    {
        var reservation = await _reservationRepository.GetAllByUserIdAsync(userId);

        return _mapper.Map<IEnumerable<ReservationResponseDto>>(reservation);
    }

    public async Task<IEnumerable<ReservationResponseDto>> GetByCurrentUser()
    {
        var reservation = await _reservationRepository.GetByUserIdAsync(UserId!);

        var activeReservations = reservation
            .Where(r => r.Status != ReservationStatus.Cancelled && r.Status != ReservationStatus.Expired && r.Status != ReservationStatus.Completed)
            .ToList();
        return _mapper.Map<IEnumerable<ReservationResponseDto>>(activeReservations);
    }

    public async Task<PagedResult<ReservationResponseDto>> GetPagedAsync(ReservationQueryParams queryParams)
    {
        queryParams.Validate();

        var query = await _reservationRepository.GetQueryable();

        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            var searchTerm = queryParams.Search.ToLower();

            query = query.Where(
                pb => pb.Place.PlaceName.ToLower().Contains(searchTerm) ||
                pb.Place.SubSection!.SubSection.ToLower().Contains(searchTerm) ||
                pb.Place.SubSection.Section!.Section.ToLower().Contains(searchTerm) ||
                pb.Place.SubSection.Section.Branch!.BranchName.ToLower().Contains(searchTerm) ||
                pb.UserId.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(queryParams.PlaceId))
        {
            var searchTerm = queryParams.Search!.ToLower();

            query = query.Where(
                pb => pb.PlaceId.ToString().ToLower() == searchTerm);
        }

        if (!string.IsNullOrWhiteSpace(queryParams.SubSectionId))
        {
            var searchTerm = queryParams.Search!.ToLower();

            query = query.Where(
                pb => pb.Place.SubSectionId.ToString().ToLower() == searchTerm);
        }

        if (!string.IsNullOrWhiteSpace(queryParams.SectionId))
        {
            var searchTerm = queryParams.Search!.ToLower();

            query = query.Where(
                pb => pb.Place.SubSection!.SectionId.ToString().ToLower() == searchTerm);
        }

        if (!string.IsNullOrWhiteSpace(queryParams.BranchId))
        {
            var searchTerm = queryParams.Search!.ToLower();

            query = query.Where(
                pb => pb.Place.SubSection!.Section!.BranchId.ToString().ToLower() == searchTerm);
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Status))
        {
            var status = queryParams.Status.ToLower();

            query = query.Where(pb => pb.Status.ToString().ToLower() == status);
        }

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(queryParams.Sort))
        {
            query = ApplySorting(query, queryParams.Sort, queryParams.SortDirection);
        }

        else
        {
            query = query.OrderByDescending(pb => pb.CreatedAt);
        }

        var skip = (queryParams.Page - 1) * queryParams.Size;
        var reservations = await query
                            .Skip(skip)
                            .Take(queryParams.Size)
                            .ToListAsync();
        var reservationDtos = _mapper.Map<IEnumerable<ReservationResponseDto>>(reservations);

        return PagedResult<ReservationResponseDto>.Create(
            reservationDtos,
            totalCount,
            queryParams.Size,
            queryParams.Page
            );
    }

    private IQueryable<ParkingReservation> ApplySorting(
        IQueryable<ParkingReservation> query,
        string sort,
        string? sortDirection)
    {
        var isDescending = sortDirection?.ToLower().Trim() == "desc";

        return sort.ToLower() switch
        {
            "username" => isDescending
                        ? query.OrderByDescending(pb => $"{pb.User.FirstName} {pb.User.LastName}")
                        : query.OrderBy(pb => $"{pb.User.FirstName} {pb.User.LastName}"),
            "useremail" => isDescending
                        ? query.OrderByDescending(pb => pb.User.Email)
                        : query.OrderBy(pb => pb.User.Email),
            "placename" => isDescending
                        ? query.OrderByDescending(pb => pb.Place.PlaceName)
                        : query.OrderBy(pb => pb.Place.PlaceName),
            "subsectionname" => isDescending
                        ? query.OrderByDescending(pb => pb.Place.SubSection!.SubSection)
                        : query.OrderBy(pb => pb.Place.SubSection!.SubSection),
            "sectionname" => isDescending
                        ? query.OrderByDescending(pb => pb.Place.SubSection!.Section!.Section)
                        : query.OrderBy(pb => pb.Place.SubSection!.Section!.Section),
            "branchname" => isDescending
                        ? query.OrderByDescending(pb => pb.Place.SubSection!.Section!.Branch!.BranchName)
                        : query.OrderBy(pb => pb.Place.SubSection!.Section!.Branch!.BranchName),
            "createdat" => isDescending
                        ? query.OrderByDescending(pb => pb.CreatedAt)
                        : query.OrderBy(pb => pb.CreatedAt),
            _ => query.OrderByDescending(pb => pb.CreatedAt)
        };
    }

    public async Task<int> GetActiveReservationsCountAsync()
    {
        var res = await _reservationRepository.GetAllAsync();

        var active = res.Where(r => r!.Status == ReservationStatus.Active).ToList();

        return active.Count();
    }
}
