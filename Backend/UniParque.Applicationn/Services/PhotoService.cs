using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using UniParque.Application.DTOs;
using UniParque.Application.Helpers;
using UniParque.Application.Repositories;
using UniParque_Domain.Entities;

namespace UniParque.Application.Services;

public class CloudinaryPhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IAppUserRepository _userRepository;
    private readonly IPhotoRepository _photoRepository;
    private readonly IMapper _mapper;

    public CloudinaryPhotoService(
        IOptions<CloudinarySettings> config,
        IHttpContextAccessor contextAccessor,
        IMapper mapper,
        IAppUserRepository userRepository,
        IPhotoRepository photoRepository)
    {
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);

        _contextAccessor = contextAccessor;
        _mapper = mapper;
        _userRepository = userRepository;
        _photoRepository = photoRepository;
    }

    private ClaimsPrincipal? User => _contextAccessor.HttpContext?.User;
    private string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public async Task<PhotoResponseDto> AddPhotoAsync(IFormFile file)
    {
        if (file.Length == 0)
            throw new Exception("No file uploaded");

        var user = await _userRepository.FindAsync(UserId!);
        if (user == null)
            throw new Exception("User not found");

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),

            Transformation = new Transformation()
                .Height(500)
                .Width(500)
                .Crop("fill")
                .Gravity("face")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            throw new Exception(uploadResult.Error.Message);

        var existing = await _photoRepository.GetByUserAsync(UserId!);

        Photo photo;

        if (existing != null)
        {
            await _cloudinary.DestroyAsync(new DeletionParams(existing.PublicId));

            existing.Url = uploadResult.SecureUrl.AbsoluteUri;
            existing.PublicId = uploadResult.PublicId;

            await _photoRepository.AddAsync(existing);
            photo = existing;
        }
        else
        {
            photo = new Photo
            {
                Url = uploadResult.SecureUrl.AbsoluteUri,
                PublicId = uploadResult.PublicId,
                UserId = UserId!,
                User = user
            };

            await _photoRepository.AddAsync(photo);
        }

        return _mapper.Map<PhotoResponseDto>(photo);
    }

    public async Task DeletePhotoAsync()
    {
        var photo = await _photoRepository.GetByUserAsync(UserId!);
        if (photo == null)
            throw new Exception("No photo found");

        await _cloudinary.DestroyAsync(
            new DeletionParams(photo.PublicId)
        );

        await _photoRepository.DeletePhotoAsync(photo.Id);
    }
}