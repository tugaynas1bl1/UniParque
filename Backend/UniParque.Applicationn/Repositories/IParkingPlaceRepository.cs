using UniParque_Domain.Entities;

namespace UniParque.Application.Repositories;

public interface IParkingPlaceRepository
{
    Task<ParkingPlace> AddAsync(ParkingPlace parkingPlace);
    Task UpdateAsync(ParkingPlace parkingPlace);
    Task RemoveAsync(ParkingPlace parkingPlace);
    Task<ParkingPlace?> GetByIdWithSubSectionAsync(Guid id);
    Task<ParkingPlace?> FindAsync(Guid id);
    Task<IEnumerable<ParkingPlace?>> GetPlacesBySubSectionIdAsync(Guid subSectionId);
    Task<IEnumerable<ParkingPlace?>> GetAllAsync();
    Task SaveChangesAsync();
}
