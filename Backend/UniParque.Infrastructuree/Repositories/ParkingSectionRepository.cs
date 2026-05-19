using Microsoft.EntityFrameworkCore;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;
using UniParque_Infrastructure.Persistence;

namespace UniParque_Infrastructure.Repositories;

public class ParkingSectionRepository : IParkingSectionRepository
{
    private readonly UniParqueDBContext _context;

    public ParkingSectionRepository(UniParqueDBContext context)
    {
        _context = context;
    }

    public async Task<ParkingSection> AddAsync(ParkingSection parkingSection)
    {
        _context.ParkingSections.Add(parkingSection);
        await _context.SaveChangesAsync();
        await _context.Entry(parkingSection).Reference(ps => ps.Branch).LoadAsync();
        return parkingSection;
    }

    public async Task<ParkingSection?> FindAsync(Guid id)
    {
        return await _context.ParkingSections.FindAsync(id);
    }

    public async Task<IEnumerable<ParkingSection>> GetAllAsync()
    {
        return await _context.ParkingSections
                            .Include(ps => ps.SubSections)
                            .Include(ps => ps.Branch)
                            .ToListAsync();
    }

    public async Task<ParkingSection?> GetByIdWithSubSectionsAndBranchesAsync(Guid id)
    {
        return await _context.ParkingSections
                .Include(ps => ps.SubSections)
                .Include(ps => ps.Branch)
                .FirstOrDefaultAsync(ps => ps.Id == id);
    }

    public async Task<ParkingSection?> GetSectionBySubSectionIdAsync(Guid subSectionId)
    {
        var subSection = await _context
            .ParkingSubSections
            .FindAsync(subSectionId);

        if (subSection is null)
            return null!;

        return await GetByIdWithSubSectionsAndBranchesAsync(subSection.SectionId);
    }

    public async Task<IEnumerable<ParkingSection?>> GetSectionsByBranchIdAsync(Guid branchId)
    {
        return await _context
            .ParkingSections
            .Include(ps => ps.Branch)
            .Where(ps => ps.BranchId == branchId)
            .ToListAsync();
    }

    public async Task RemoveAsync(ParkingSection parkingSection)
    {
        _context.ParkingSections.Remove(parkingSection);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ParkingSection parkingSection)
    {
        _context.ParkingSections.Update(parkingSection);
        await _context.SaveChangesAsync();
    }
}
