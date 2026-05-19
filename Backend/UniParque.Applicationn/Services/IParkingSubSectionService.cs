using UniParque.Application.DTOs;

namespace UniParque.Application.Services;

public interface IParkingSubSectionService
{
    Task<ParkingSubSectionResponseDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ParkingSubSectionResponseDto>> GetAllAsync();
    Task<IEnumerable<ParkingSubSectionResponseDto>> GetBySectionIdAsync(Guid sectionId);
    Task<ParkingSubSectionResponseDto> CreateAsync(CreateParkingSubSectionRequestDto createdSubSectionRequest);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeleteAllAsync();
    Task<ParkingSubSectionResponseDto> UpdateAsync(Guid id, UpdateParkingSubSectionRequestDto updatedSubSectionRequest);
}
