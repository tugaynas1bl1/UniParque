using Microsoft.EntityFrameworkCore;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;
using UniParque_Infrastructure.Persistence;

namespace UniParque_Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly UniParqueDBContext _context;

    public ReservationRepository(UniParqueDBContext context)
    {
        _context = context;
    }

    public async Task<ParkingReservation> AddAsync(ParkingReservation parkingReservation)
    {
        await _context
            .Reservations
            .AddAsync(parkingReservation);

        await _context.SaveChangesAsync();

        var reservation = await _context
                                 .Reservations
                                 .Include(r => r.User)
                                 .Include(r => r.Place)
                                    .ThenInclude(p => p.SubSection)
                                        .ThenInclude(ss => ss!.Section)
                                            .ThenInclude(s => s!.Branch)
                                 .FirstOrDefaultAsync(r => r.Id == parkingReservation.Id);        

        return reservation!;
    }

    public async Task<ParkingReservation?> FindAsync(Guid id)
    {
        return await _context
                    .Reservations
                    .FindAsync(id);
    }

    public async Task<IEnumerable<ParkingReservation?>> GetAllAsync()
    {
        return await _context
                    .Reservations
                    .Include(pr => pr.User)
                    .Include(pr => pr.Place)
                        .ThenInclude(p => p.SubSection)
                            .ThenInclude(ss => ss!.Section)
                                .ThenInclude(s => s!.Branch)
                    .ToListAsync();
    }

    public async Task<ParkingReservation?> GetByCarNumber(string carNumb)
    {
        return await _context
                    .Reservations
                    .Include(pr => pr.User)
                    .Include(pr => pr.Place)
                        .ThenInclude(p => p.SubSection)
                            .ThenInclude(ss => ss!.Section)
                                .ThenInclude(s => s!.Branch)
                    .FirstOrDefaultAsync(r => r.CarNumber == carNumb && (r.Status == ReservationStatus.Active || r.Status == ReservationStatus.CheckedIn));
    }

    public async Task<ParkingReservation?> GetByIdWithUsersAndPlacesAsync(Guid id)
    {
        return await _context
                    .Reservations
                    .Include(pr => pr.User)
                    .Include(pr => pr.Place)
                        .ThenInclude(p => p.SubSection)
                            .ThenInclude(ss => ss!.Section)
                                .ThenInclude(s => s!.Branch)
                    .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<ParkingReservation?> GetByPlaceIdAsync(Guid placeId)
    {
        return await _context
                    .Reservations
                    .Include(pr => pr.User)
                    .Include(pr => pr.Place)
                        .ThenInclude(p => p.SubSection)
                            .ThenInclude(ss => ss!.Section)
                                .ThenInclude(s => s!.Branch)
                    .FirstOrDefaultAsync(r => r.PlaceId == placeId);
    }

    public async Task<IEnumerable<ParkingReservation?>> GetByUserIdAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);

        return await _context
                     .Reservations
                     .Include(pr => pr.User)
                     .Include(pr => pr.Place)
                        .ThenInclude(p => p.SubSection)
                            .ThenInclude(ss => ss!.Section)
                                .ThenInclude(s => s!.Branch)
                     .Where(r => r.UserId == userId && (r.Status != ReservationStatus.Cancelled && r.Status != ReservationStatus.Expired && r.Status != ReservationStatus.Completed))
                     .ToListAsync();
    }

    public async Task<IEnumerable<ParkingReservation?>> GetAllByUserIdAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);

        return await _context
                     .Reservations
                     .Include(pr => pr.User)
                     .Include(pr => pr.Status)
                     .Include(pr => pr.Place)
                        .ThenInclude(p => p.SubSection)
                            .ThenInclude(ss => ss!.Section)
                                .ThenInclude(s => s!.Branch)
                     .Where(r => r.UserId == userId)
                     .ToListAsync();
    }

    public async Task RemoveAsync(ParkingReservation parkingReservation)
    {
        _context.Reservations.Remove(parkingReservation);
        await _context.SaveChangesAsync();
    }

    public async Task<IQueryable<ParkingReservation>> GetQueryable()
    {
        return _context.Reservations
                .Include(r => r.Place)
                    .ThenInclude(p => p.SubSection)
                        .ThenInclude(ss => ss!.Section)
                            .ThenInclude(s => s!.Branch)
                .Include(r => r.User)
                .AsQueryable();
    }
    public async Task SetStatus(ParkingReservation reservation, ReservationStatus status)
    {
        reservation.Status = status;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ParkingReservation parkingReservation)
    {
        _context.Reservations.Update(parkingReservation);
        await _context.SaveChangesAsync();
    }
}
