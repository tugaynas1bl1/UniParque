using Microsoft.EntityFrameworkCore;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;
using UniParque_Infrastructure.Persistence;

namespace UniParque_Infrastructure.Repositories;

public class ParkingPlaceRepository : IParkingPlaceRepository
{
    private readonly UniParqueDBContext _context;

    public ParkingPlaceRepository(UniParqueDBContext context)
    {
        _context = context;
    }

    public async Task<ParkingPlace> AddAsync(ParkingPlace parkingPlace)
    {
        _context.ParkingPlaces.Add(parkingPlace);
        await _context.SaveChangesAsync();
        await _context.Entry(parkingPlace).Reference(pp => pp.SubSection).LoadAsync();
        return parkingPlace;
    }

    public async Task<ParkingPlace?> FindAsync(Guid id)
    {
        return await _context.ParkingPlaces.FindAsync(id);
    }

    public async Task<IEnumerable<ParkingPlace?>> GetAllAsync()
    {
        return await _context.ParkingPlaces
                            .Include(pp => pp.SubSection)
                            .ToListAsync();
    }

    public async Task<ParkingPlace?> GetByIdWithSubSectionAsync(Guid id)
    {
        return await _context.ParkingPlaces
                .Include(pp => pp.SubSection)
                .FirstOrDefaultAsync(pp => pp.Id == id);
    }

    public async Task<IEnumerable<ParkingPlace?>> GetPlacesBySubSectionIdAsync(Guid subSectionId)
    {
        return await _context
            .ParkingPlaces
            .Include(pp => pp.SubSection)
            .Where(pp => pp.SubSectionId == subSectionId)
            .ToListAsync();
    }

    public async Task RemoveAsync(ParkingPlace parkingPlace)
    {
        _context.ParkingPlaces.Remove(parkingPlace);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ParkingPlace parkingPlace)
    {
        _context.ParkingPlaces.Update(parkingPlace);
        await _context.SaveChangesAsync();
    }
}
