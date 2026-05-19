using UniParque.Application.DTOs;

namespace UniParque.Application.Services;

public interface IParkingSectionService
{
    Task<ParkingSectionResponseDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ParkingSectionResponseDto>> GetAllAsync();
    Task<IEnumerable<ParkingSectionResponseDto>> GetByBranchIdAsync(Guid branchId);
    Task<ParkingSectionResponseDto?> CreateAsync(CreateParkingSectionRequestDto createdSectionRequest);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeleteAllAsync();
    Task<ParkingSectionResponseDto?> UpdateAsync(Guid id, UpdateParkingSectionRequestDto updatedSectionRequest);
}
