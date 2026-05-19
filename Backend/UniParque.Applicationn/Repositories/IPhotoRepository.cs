using CloudinaryDotNet.Actions;
using UniParque_Domain.Entities;

namespace UniParque.Application.Repositories;

public interface IPhotoRepository
{
    Task<Photo> AddAsync(Photo photo);
    Task<Photo> GetByUserAsync(string userId);
    Task ChangePhotoAsync(Photo photo);
    Task DeletePhotoAsync(Guid id);
}
