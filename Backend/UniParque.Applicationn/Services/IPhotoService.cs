using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using UniParque.Application.DTOs;

namespace UniParque.Application.Services;

public interface IPhotoService
{
    Task<PhotoResponseDto> AddPhotoAsync(IFormFile file);
    Task DeletePhotoAsync();
}
