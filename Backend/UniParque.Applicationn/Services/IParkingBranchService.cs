using UniParque.Application.Common;
using UniParque.Application.DTOs;

namespace UniParque.Application.Services;

public interface IParkingBranchService
{
    Task<ParkingBranchResponseDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ParkingBranchResponseDto>> GetAllAsync();
    Task<ParkingBranchResponseDto> CreateAsync(CreateParkingBranchRequestDto createdBranchRequest);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeleteAllAsync();
    Task<ParkingBranchResponseDto> UpdateAsync(Guid id, UpdateParkingBranchRequestDto updatedBranchRequest);
    Task<PagedResult<ParkingBranchResponseDto>> GetPagedAsync(ParkingBranchQueryParams queryParams);
    Task<ParkingBranchFullLayoutDto> GetFullLayoutByIdAsync(Guid id);
}
