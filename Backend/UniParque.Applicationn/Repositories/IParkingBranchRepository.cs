using UniParque_Domain.Entities;

namespace UniParque.Application.Repositories;

public interface IParkingBranchRepository
{
    Task<ParkingBranch> AddAsync(ParkingBranch parkingBranch);
    Task UpdateAsync(ParkingBranch parkingBranch);
    Task RemoveAsync(ParkingBranch parkingBranch);
    Task<ParkingBranch?> GetByIdWithSectionsAsync (Guid id);
    Task<ParkingBranch?> FindAsync(Guid id);
    Task<ParkingBranch?> GetBySectionIdAsync (Guid sectionId);
    Task<IEnumerable<ParkingBranch>> GetAllAsync();
    Task<IQueryable<ParkingBranch>> GetQueryable();
    Task<ParkingBranch?> GetFullLayout(Guid id);
}
