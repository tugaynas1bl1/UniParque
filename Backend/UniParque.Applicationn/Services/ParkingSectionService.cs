using AutoMapper;
using UniParque.Application.Common;
using UniParque.Application.DTOs;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;

namespace UniParque.Application.Services;

public class ParkingSectionService : IParkingSectionService
{
    private readonly IParkingSectionRepository _parkingSectionRepository;
    private readonly IMapper _mapper;

    public ParkingSectionService(IParkingSectionRepository sectionRepository, IMapper mapper)
    {
        _parkingSectionRepository = sectionRepository;
        _mapper = mapper;
    }

    public async Task<ParkingSectionResponseDto> CreateAsync(CreateParkingSectionRequestDto createdSectionRequest)
    {
        var createdSection = _mapper.Map<ParkingSection>(createdSectionRequest);

        await _parkingSectionRepository.AddAsync(createdSection);

        return _mapper.Map<ParkingSectionResponseDto>(createdSection);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var section = await _parkingSectionRepository.FindAsync(id);

        if (section is null)
            return false;

        await _parkingSectionRepository.RemoveAsync(section);
        return true;
    }

    public async Task<bool> DeleteAllAsync()
    {
        var allSections = await _parkingSectionRepository.GetAllAsync();

        if (allSections is null || !allSections.Any())
            return false;

        foreach (var section in allSections)
        {
            await _parkingSectionRepository.RemoveAsync(section);
        }

        return true;
    }

    public async Task<IEnumerable<ParkingSectionResponseDto>> GetAllAsync()
    {
        var sectiones = await _parkingSectionRepository.GetAllAsync();

        return sectiones.Count() == 0 ? null! : _mapper.Map<IEnumerable<ParkingSectionResponseDto>>(sectiones);
    }

    public async Task<ParkingSectionResponseDto> GetByIdAsync(Guid id)
    {
        var section = await _parkingSectionRepository.GetByIdWithSubSectionsAndBranchesAsync(id);

        return section is null ? null! : _mapper.Map<ParkingSectionResponseDto>(section);
    }

    public async Task<ParkingSectionResponseDto> UpdateAsync(Guid id, UpdateParkingSectionRequestDto updatedSectionRequest)
    {
        var updatedSection = await _parkingSectionRepository.GetByIdWithSubSectionsAndBranchesAsync(id);

        if (updatedSection is null)
            throw new NullReferenceException();

        _mapper.Map(updatedSectionRequest, updatedSection);

        await _parkingSectionRepository.UpdateAsync(updatedSection);

        return _mapper.Map<ParkingSectionResponseDto>(updatedSection);
    }

    public async Task<IEnumerable<ParkingSectionResponseDto>> GetByBranchIdAsync(Guid branchId)
    {
        var sectionsByBranch = await _parkingSectionRepository.GetSectionsByBranchIdAsync(branchId);

        if (sectionsByBranch is null || !sectionsByBranch.Any())
            return null!;

        return _mapper.Map<IEnumerable<ParkingSectionResponseDto>>(sectionsByBranch);
    }
}
