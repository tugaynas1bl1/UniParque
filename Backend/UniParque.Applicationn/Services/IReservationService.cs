using UniParque.Application.Common;
using UniParque.Application.DTOs;

namespace UniParque.Application.Services;

public interface IReservationService
{
    Task<ReservationResponseDto> GetByIdAsync(Guid id);
    Task<int> GetActiveReservationsCountAsync();
    Task<IEnumerable<ReservationResponseDto>> GetAllAsync();
    Task<IEnumerable<ReservationResponseDto>> GetByUserIdAsync(string userId);
    Task<decimal> CalculatePriceAsync(CalculatePriceRequestDto calculateRequest);
    Task<ReservationResponseDto> CreateAsync(CreateReservationWithCardRequestDto createdReservationRequest);
    Task<ReservationResponseDto> CreateByBalanceAsync(CreateReservationRequestDto createdReservationRequest);
    Task<ReservationResponseDto> CreateWithSpecificUserAsync(CreateReservationRequestWithSpecificUserDto createdReservationRequest);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeleteAllAsync();
    Task<ReservationResponseDto> UpdateAsync(Guid id, UpdateReservationRequestDto updatedReservationRequest);
    Task<bool> ConfirmArrivalAsync(Guid reservationId);
    Task<bool> EndReservationSessionAndLeaveAsync(Guid reservationId);
    Task<bool> CancelReservationAsync(Guid reservationId);
    Task<IEnumerable<ReservationResponseDto>> GetByUserId(string userId);
    Task<IEnumerable<ReservationResponseDto>> GetByCurrentUser();
    Task<PagedResult<ReservationResponseDto>> GetPagedAsync(ReservationQueryParams queryParams);

}
