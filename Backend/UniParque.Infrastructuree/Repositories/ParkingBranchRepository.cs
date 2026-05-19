using Microsoft.EntityFrameworkCore;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;
using UniParque_Infrastructure.Persistence;

namespace UniParque_Infrastructure.Repositories;

public class ParkingBranchRepository : IParkingBranchRepository
{
    private readonly UniParqueDBContext _context;

    public ParkingBranchRepository(UniParqueDBContext context)
    {
        _context = context;
    }

    public async Task<ParkingBranch> AddAsync(ParkingBranch parkingBranch)
    {
        _context.ParkingBranches.Add(parkingBranch);
        await _context.SaveChangesAsync();
        return parkingBranch;
    }

    public async Task<ParkingBranch?> FindAsync(Guid id)
    {
        return await _context.ParkingBranches.FindAsync(id);
    }

    public async Task<IEnumerable<ParkingBranch>> GetAllAsync()
    {
        return await _context
                        .ParkingBranches
                        .Include(pb => pb.Sections)
                        .ToListAsync();
    }

    public async Task<ParkingBranch?> GetByIdWithSectionsAsync(Guid id)
    {
        return await _context
            .ParkingBranches
            .Include(pb => pb.Sections)
            .FirstOrDefaultAsync(pb => pb.Id == id);
    }

    public async Task<ParkingBranch?> GetBySectionIdAsync(Guid sectionId)
    {
        var section = await _context.ParkingSections
            .FindAsync(sectionId);

        if (section is null)
            return null!;

        return await GetByIdWithSectionsAsync(section.BranchId);
    }

    public async Task RemoveAsync(ParkingBranch parkingBranch)
    {
        _context.ParkingBranches.Remove(parkingBranch);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ParkingBranch parkingBranch)
    {
        _context.ParkingBranches.Update(parkingBranch);
        await _context.SaveChangesAsync();
    }

    public async Task<IQueryable<ParkingBranch>> GetQueryable()
    {
        return _context.ParkingBranches
                .Include(pb => pb.Sections)
                .AsQueryable();
    }

    public async Task<ParkingBranch?> GetFullLayout(Guid id)
    {
        var branch = await _context.ParkingBranches
                    .Include(b => b.Sections)
                        .ThenInclude(s => s.SubSections)
                            .ThenInclude(ss => ss.Places)
                                .ThenInclude(p => p.Reservations)
                    .FirstOrDefaultAsync(b => b.Id == id);
        return branch;
    }
}
