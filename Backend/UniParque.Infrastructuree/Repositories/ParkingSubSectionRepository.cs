using Microsoft.EntityFrameworkCore;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;
using UniParque_Infrastructure.Persistence;

namespace UniParque_Infrastructure.Repositories;

public class ParkingSubSectionRepository : IParkingSubSectionRepository
{
    private readonly UniParqueDBContext _context;

    public ParkingSubSectionRepository(UniParqueDBContext context)
    {
        _context = context;
    }

    public async Task<ParkingSubSection> AddAsync(ParkingSubSection parkingSubSection)
    {
        _context.ParkingSubSections.Add(parkingSubSection);
        await _context.SaveChangesAsync();
        await _context.Entry(parkingSubSection).Reference(pss => pss.Section).LoadAsync();
        return parkingSubSection;
    }

    public async Task<ParkingSubSection?> FindAsync(Guid id)
    {
        return await _context.ParkingSubSections.FindAsync(id);
    }

    public async Task<IEnumerable<ParkingSubSection?>> GetAllAsync()
    {
        return await _context.ParkingSubSections
                            .Include(pss => pss.Places)
                            .Include(pss => pss.Section)
                            .ToListAsync();
    }

    public async Task<ParkingSubSection?> GetByIdWithPlacesAndSectionAsync(Guid id)
    {
        return await _context.ParkingSubSections
                .Include(pss => pss.Places)
                .Include(pss => pss.Section)
                .FirstOrDefaultAsync(ps => ps.Id == id);
    }

    public async Task<ParkingSubSection?> GetSubSectionByPlaceIdAsync(Guid placeId)
    {
        var place = await _context
            .ParkingPlaces
            .FindAsync(placeId);

        if (place is null)
            return null!;

        return await GetByIdWithPlacesAndSectionAsync(place.SubSectionId);
    }

    public async Task<IEnumerable<ParkingSubSection?>> GetSubSectionsBySectionIdAsync(Guid sectionId)
    {
        return await _context
            .ParkingSubSections
            .Include(pss => pss.Section)
            .Where(pss => pss.SectionId == sectionId)
            .ToListAsync();
    }

    public async Task RemoveAsync(ParkingSubSection parkingSubSection)
    {
        _context.ParkingSubSections.Remove(parkingSubSection);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ParkingSubSection parkingSubSection)
    {
        _context.ParkingSubSections.Update(parkingSubSection);
        await _context.SaveChangesAsync();
    }
}
