using UniParque_Domain.Entities;

namespace UniParque.Application.Repositories;

public interface IReservationRepository
{
    Task<ParkingReservation> AddAsync(ParkingReservation parkingReservation);
    Task UpdateAsync(ParkingReservation parkingReservation);
    Task RemoveAsync(ParkingReservation parkingReservation);
    Task<ParkingReservation?> FindAsync(Guid id);
    Task<ParkingReservation?> GetByIdWithUsersAndPlacesAsync(Guid id);
    Task<IEnumerable<ParkingReservation?>> GetByUserIdAsync(string userId);
    Task<ParkingReservation?> GetByPlaceIdAsync(Guid placeId);
    Task<ParkingReservation?> GetByCarNumber(string carNumb);
    Task<IEnumerable<ParkingReservation?>> GetAllAsync();
    Task<IEnumerable<ParkingReservation?>> GetAllByUserIdAsync(string userId);
    Task SetStatus(ParkingReservation reservation, ReservationStatus status);
    Task<IQueryable<ParkingReservation>> GetQueryable();
}
