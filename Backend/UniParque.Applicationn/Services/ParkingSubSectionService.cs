using AutoMapper;
using UniParque.Application.DTOs;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;

namespace UniParque.Application.Services;

public class ParkingSubSectionService : IParkingSubSectionService
{
    private readonly IParkingSubSectionRepository _parkingSubSectionRepository;
    private readonly IMapper _mapper;

    public ParkingSubSectionService(IParkingSubSectionRepository parkingSubSectionRepository, IMapper mapper)
    {
        _parkingSubSectionRepository = parkingSubSectionRepository;
        _mapper = mapper;
    }

    public async Task<ParkingSubSectionResponseDto> CreateAsync(CreateParkingSubSectionRequestDto createdSubSectionRequest)
    {
        var createdSubSection = _mapper.Map<ParkingSubSection>(createdSubSectionRequest);

        await _parkingSubSectionRepository.AddAsync(createdSubSection);

        return _mapper.Map<ParkingSubSectionResponseDto>(createdSubSection);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var subSection = await _parkingSubSectionRepository.FindAsync(id);

        if (subSection is null)
            return false;

        await _parkingSubSectionRepository.RemoveAsync(subSection);
        return true;
    }

    public async Task<bool> DeleteAllAsync()
    {
        var allSubSections = await _parkingSubSectionRepository.GetAllAsync();

        if (allSubSections is null || !allSubSections.Any())
            return false;

        foreach (var subSection in allSubSections)
        {
            await _parkingSubSectionRepository.RemoveAsync(subSection!);
        }

        return true;
    }

    public async Task<IEnumerable<ParkingSubSectionResponseDto>> GetAllAsync()
    {
        var subSections = await _parkingSubSectionRepository.GetAllAsync();

        return subSections.Count() == 0 ? null! : _mapper.Map<IEnumerable<ParkingSubSectionResponseDto>>(subSections);
    }

    public async Task<ParkingSubSectionResponseDto> GetByIdAsync(Guid id)
    {
        var subSection = await _parkingSubSectionRepository.GetByIdWithPlacesAndSectionAsync(id);

        return subSection is null ? null! : _mapper.Map<ParkingSubSectionResponseDto>(subSection);
    }

    public async Task<ParkingSubSectionResponseDto> UpdateAsync(Guid id, UpdateParkingSubSectionRequestDto updatedSubSectionRequest)
    {
        var updatedSubSection = await _parkingSubSectionRepository.GetByIdWithPlacesAndSectionAsync(id);

        if (updatedSubSection is null)
            throw new NullReferenceException();

        _mapper.Map(updatedSubSectionRequest, updatedSubSection);

        await _parkingSubSectionRepository.UpdateAsync(updatedSubSection);

        return _mapper.Map<ParkingSubSectionResponseDto>(updatedSubSection);
    }

    public async Task<IEnumerable<ParkingSubSectionResponseDto>> GetBySectionIdAsync(Guid sectionId)
    {
        var subSectionsBySection = await _parkingSubSectionRepository.GetSubSectionsBySectionIdAsync(sectionId);

        if (subSectionsBySection is null || !subSectionsBySection.Any())
            return null!;

        return _mapper.Map<IEnumerable<ParkingSubSectionResponseDto>>(subSectionsBySection);
    }
}
