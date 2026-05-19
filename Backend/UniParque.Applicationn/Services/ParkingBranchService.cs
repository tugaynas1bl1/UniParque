using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniParque.Application.Common;
using UniParque.Application.DTOs;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;
using static System.Collections.Specialized.BitVector32;

namespace UniParque.Application.Services;

public class ParkingBranchService : IParkingBranchService
{
    private readonly IParkingBranchRepository _parkingBranchRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ParkingBranchService(IParkingBranchRepository parkingBranchRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _parkingBranchRepository = parkingBranchRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ParkingBranchResponseDto> CreateAsync(CreateParkingBranchRequestDto createdBranchRequest)
    {
        var createdBranch = _mapper.Map<ParkingBranch>(createdBranchRequest);

        await _parkingBranchRepository.AddAsync(createdBranch);

        return _mapper.Map<ParkingBranchResponseDto>(createdBranch);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var branch = await _parkingBranchRepository.FindAsync(id);

        if (branch is null)
            return false;

        await _parkingBranchRepository.RemoveAsync(branch);
        return true;
    }

    public async Task<bool> DeleteAllAsync()
    {
        var allBranches = await _parkingBranchRepository.GetAllAsync();

        if (allBranches is null || !allBranches.Any())
            return false;

        foreach (var branch in allBranches)
        {
            await _parkingBranchRepository.RemoveAsync(branch);
        }

        return true;
    }

    public async Task<IEnumerable<ParkingBranchResponseDto>> GetAllAsync()
    {
        var branches = await _parkingBranchRepository.GetAllAsync();

        return branches.Count() == 0 ? null! : _mapper.Map<IEnumerable<ParkingBranchResponseDto>>(branches);
    }

    public async Task<ParkingBranchResponseDto> GetByIdAsync(Guid id)
    {
        var branch = await _parkingBranchRepository.GetByIdWithSectionsAsync(id);

        return branch is null ? null! : _mapper.Map<ParkingBranchResponseDto>(branch);
    }

    public async Task<ParkingBranchResponseDto> UpdateAsync(Guid id, UpdateParkingBranchRequestDto updatedBranchRequest)
    {
        var updatedBranch = await _parkingBranchRepository.GetByIdWithSectionsAsync(id);

        if (updatedBranch is null)
            throw new NullReferenceException();

        _mapper.Map(updatedBranchRequest, updatedBranch);

        await _parkingBranchRepository.UpdateAsync(updatedBranch);

        return _mapper.Map<ParkingBranchResponseDto>(updatedBranch);
    }

    public async Task<PagedResult<ParkingBranchResponseDto>> GetPagedAsync(ParkingBranchQueryParams queryParams)
    {
        queryParams.Validate();

        var query = await _parkingBranchRepository.GetQueryable();

        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            var searchTerm = queryParams.Search.ToLower();

            query = query.Where(
                pb => pb.BranchName.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(queryParams.Sort))
        {
            query = ApplySorting(query, queryParams.Sort, queryParams.SortDirection);
        }
        else
        {
            query = query.OrderByDescending(pb => pb.CreatedAt);
        }

        var skip = (queryParams.Page - 1) * queryParams.Size;
        var branches = await query
                            .Skip(skip)
                            .Take(queryParams.Size)
                            .ToListAsync();
        var branchDtos = _mapper.Map<IEnumerable<ParkingBranchResponseDto>>(branches);

        return PagedResult<ParkingBranchResponseDto>.Create(
            branchDtos,
            totalCount,
            queryParams.Size,
            queryParams.Page
            );
    }

    private IQueryable<ParkingBranch> ApplySorting(
        IQueryable<ParkingBranch> query,
        string sort,
        string? sortDirection)
    {
        var isDescending = sortDirection?.ToLower().Trim() == "desc";

        return sort.ToLower() switch
        {
            "branchname" => isDescending
                        ? query.OrderByDescending(pb => pb.BranchName)
                        : query.OrderBy(pb => pb.BranchName),
            "createdat" => isDescending
                        ? query.OrderByDescending(pb => pb.CreatedAt)
                        : query.OrderBy(pb => pb.CreatedAt),
            _ => query.OrderByDescending(pb => pb.CreatedAt)    
        };
    }

    public async Task<ParkingBranchFullLayoutDto> GetFullLayoutByIdAsync(Guid id)
    {
        var branch = await _parkingBranchRepository.GetFullLayout(id);

        var sections = branch!.Sections ?? new List<ParkingSection>();

        var currentUserId = _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

        return new ParkingBranchFullLayoutDto
        {
            Id = branch.Id,
            BranchName = branch.BranchName,
            Sections = sections.Select(s => new SectionDto
            {
                Id = s.Id,
                Name = s.Section,
                SubSections = (s.SubSections ?? new List<ParkingSubSection>())
                    .Select(ss => new SubSectionDto
                    {
                        Id = ss.Id,
                        Name = ss.SubSection,
                        Places = (ss.Places ?? new List<ParkingPlace>())
                            .Select(p => new PlaceDto
                            {
                                Id = p.Id,
                                IsReserved = p.IsReserved,
                                IsOccupied = p.IsTaken,

                                IsReservedByMe = p.Reservations
                                    .Any(r => r.UserId == currentUserId && (r.Status == ReservationStatus.Active || r.Status == ReservationStatus.CheckedIn)),
                            }).ToList()
                    }).ToList()
            }).ToList()
        };
    }
}
