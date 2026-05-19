using UniParque_Domain.Entities;

namespace UniParque.Application.Repositories;

public interface IParkingSubSectionRepository
{
    Task<ParkingSubSection> AddAsync(ParkingSubSection parkingSubSection);
    Task UpdateAsync(ParkingSubSection parkingSubSection);
    Task RemoveAsync(ParkingSubSection parkingSubSection);
    Task<ParkingSubSection?> GetByIdWithPlacesAndSectionAsync(Guid id);
    Task<ParkingSubSection?> FindAsync(Guid id);
    Task<IEnumerable<ParkingSubSection?>> GetSubSectionsBySectionIdAsync(Guid sectionId);
    Task<ParkingSubSection?> GetSubSectionByPlaceIdAsync(Guid placeId);
    Task<IEnumerable<ParkingSubSection?>> GetAllAsync();
}
