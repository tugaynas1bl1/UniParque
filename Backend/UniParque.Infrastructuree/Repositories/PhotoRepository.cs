using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;
using UniParque_Infrastructure.Persistence;

namespace UniParque_Infrastructure.Repositories;

public class PhotoRepository : IPhotoRepository
{
    private readonly UniParqueDBContext _context;

    public PhotoRepository(UniParqueDBContext context)
        =>  _context = context;

    public async Task<Photo> AddAsync(Photo photo)
    {
        _context.Photos.Add(photo);
        await _context.Entry(photo).Reference(p => p.User).LoadAsync();
        await _context.SaveChangesAsync();
        return photo;
    }

    public async Task<Photo> GetByUserAsync(string userId)
    {
        var photo = await _context
                            .Photos
                            .Include(p => p.User)
                            .FirstOrDefaultAsync(p =>  p.UserId == userId);
        return photo!;
    }
    public async Task ChangePhotoAsync(Photo photo)
    {
        var existing = await _context.Photos.FindAsync(photo.Id);

        if (existing == null)
            throw new Exception("Photo not found");

        existing.Url = photo.Url;
        existing.PublicId = photo.PublicId;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Photo photo)
    {
        _context.Photos.Update(photo);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePhotoAsync(Guid id)
    {
        var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
        if (photo == null) return;

        _context.Photos.Remove(photo!);
        await _context.SaveChangesAsync();
    }
}
