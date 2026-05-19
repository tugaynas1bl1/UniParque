using UniParque.Application.DTOs;

namespace UniParque.Application.Services;

public interface IParkingPlaceService
{
    Task<ParkingPlaceResponseDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ParkingPlaceResponseDto>> GetAllAsync();
    Task<IEnumerable<ParkingPlaceResponseDto>> GetBySubSectionIdAsync(Guid subSectionId);
    Task<ParkingPlaceResponseDto> CreateAsync(CreateParkingPlaceRequestDto createdPlaceRequest);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeleteAllAsync();
    Task<ParkingPlaceResponseDto> UpdateAsync(Guid id, UpdateParkingPlaceRequestDto updatedPlaceRequest);
}
