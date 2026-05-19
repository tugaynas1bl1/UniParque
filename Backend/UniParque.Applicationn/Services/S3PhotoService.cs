using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using UniParque.Application.DTOs;
using UniParque.Application.Repositories;
using UniParque.Application.Services;
using UniParque_Domain.Entities;

public class S3PhotoService : IPhotoService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _region;

    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IAppUserRepository _userRepository;
    private readonly IPhotoRepository _photoRepository;
    private readonly IMapper _mapper;

    public S3PhotoService(
        IConfiguration configuration,
        IHttpContextAccessor contextAccessor,
        IMapper mapper,
        IAppUserRepository userRepository,
        IPhotoRepository photoRepository)
    {
        var aws = configuration.GetSection("AWS");

        var credentials = new Amazon.Runtime.BasicAWSCredentials(
            aws["AccessKey"], aws["SecretKey"]);

        _region = aws["Region"]!;
        _bucketName = aws["BucketName"]!;

        _s3Client = new AmazonS3Client(credentials,
            new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_region)
            });

        _contextAccessor = contextAccessor;
        _mapper = mapper;
        _userRepository = userRepository;
        _photoRepository = photoRepository;
    }

    private string? UserId =>
        _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    public async Task<PhotoResponseDto> AddPhotoAsync(IFormFile file)
    {
        if (file.Length == 0)
            throw new Exception("No file uploaded");

        var user = await _userRepository.FindAsync(UserId!);
        if (user == null)
            throw new Exception("User not found");

        var key = $"profiles/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        using var stream = file.OpenReadStream();

        await _s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType
        });

        var url = $"https://{_bucketName}.s3.{_region}.amazonaws.com/{key}";

        var existing = await _photoRepository.GetByUserAsync(UserId!);

        Photo photo;

        if (existing != null)
        {
            await DeleteFromS3(existing.PublicId);

            existing.Url = url;
            existing.PublicId = key;

            await _photoRepository.ChangePhotoAsync(existing);
            photo = existing;
        }
        else
        {
            photo = new Photo
            {
                Url = url,
                PublicId = key,
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
        if (photo == null) throw new Exception("No photo");

        await DeleteFromS3(photo.PublicId);
        await _photoRepository.DeletePhotoAsync(photo.Id);
    }

    private async Task DeleteFromS3(string key)
    {
        await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        });
    }
}