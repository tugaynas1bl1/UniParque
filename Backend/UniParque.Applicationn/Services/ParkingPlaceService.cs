using AutoMapper;
using UniParque.Application.DTOs;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;

namespace UniParque.Application.Services;

public class ParkingPlaceService : IParkingPlaceService
{
    private readonly IParkingPlaceRepository _parkingPlaceRepository;
    private readonly IMapper _mapper;

    public ParkingPlaceService(IParkingPlaceRepository parkingPlaceRepository, IMapper mapper)
    {
        _parkingPlaceRepository = parkingPlaceRepository;
        _mapper = mapper;
    }

    public async Task<ParkingPlaceResponseDto> CreateAsync(CreateParkingPlaceRequestDto createdPlaceRequest)
    {
        var createdPlace = _mapper.Map<ParkingPlace>(createdPlaceRequest);

        await _parkingPlaceRepository.AddAsync(createdPlace);

        return _mapper.Map<ParkingPlaceResponseDto>(createdPlace);
    }

    public async Task<bool> DeleteAllAsync()
    {
        var allPlaces = await _parkingPlaceRepository.GetAllAsync();

        if (allPlaces is null || !allPlaces.Any())
            return false;

        foreach (var place in allPlaces)
        {
            await _parkingPlaceRepository.RemoveAsync(place!);
        }

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var place = await _parkingPlaceRepository.FindAsync(id);

        if (place is null)
            return false;

        await _parkingPlaceRepository.RemoveAsync(place);
        return true;
    }

    public async Task<IEnumerable<ParkingPlaceResponseDto>> GetAllAsync()
    {
        var places = await _parkingPlaceRepository.GetAllAsync();

        return places.Count() == 0 ? null! : _mapper.Map<IEnumerable<ParkingPlaceResponseDto>>(places);
    }

    public async Task<ParkingPlaceResponseDto> GetByIdAsync(Guid id)
    {
        var place = await _parkingPlaceRepository.GetByIdWithSubSectionAsync(id);

        return place is null ? null! : _mapper.Map<ParkingPlaceResponseDto>(place);
    }

    public async Task<IEnumerable<ParkingPlaceResponseDto>> GetBySubSectionIdAsync(Guid subSectionId)
    {
        var places = await _parkingPlaceRepository.GetPlacesBySubSectionIdAsync(subSectionId);

        if (places is null || !places.Any())
            return null!;

        return _mapper.Map<IEnumerable<ParkingPlaceResponseDto>>(places);
    }

    public async Task<ParkingPlaceResponseDto> UpdateAsync(Guid id, UpdateParkingPlaceRequestDto updatedPlaceRequest)
    {
        var updatedPlace = await _parkingPlaceRepository.GetByIdWithSubSectionAsync(id);

        if (updatedPlace is null)
            throw new NullReferenceException();

        _mapper.Map(updatedPlaceRequest, updatedPlace);

        await _parkingPlaceRepository.UpdateAsync(updatedPlace);

        return _mapper.Map<ParkingPlaceResponseDto>(updatedPlace);
    }
}
