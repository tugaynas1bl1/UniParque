using UniParque_Domain.Entities;

namespace UniParque.Application.Repositories;

public interface IParkingSectionRepository
{
    Task<ParkingSection> AddAsync(ParkingSection parkingSection);
    Task UpdateAsync(ParkingSection parkingSection);
    Task RemoveAsync(ParkingSection parkingSection);
    Task<ParkingSection?> GetByIdWithSubSectionsAndBranchesAsync(Guid id);
    Task<ParkingSection?> FindAsync(Guid id);
    Task<IEnumerable<ParkingSection?>> GetSectionsByBranchIdAsync(Guid branchId);
    Task<ParkingSection?> GetSectionBySubSectionIdAsync(Guid subSectionId);
    Task<IEnumerable<ParkingSection>> GetAllAsync();
}
