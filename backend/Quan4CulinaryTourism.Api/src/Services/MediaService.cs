using Microsoft.AspNetCore.Http;
using Quan4CulinaryTourism.Api.Helpers;
using Quan4CulinaryTourism.Api.Models;
using Quan4CulinaryTourism.Api.Repositories;

namespace Quan4CulinaryTourism.Api.Services;

public class MediaService
{
    private readonly FileUploadHelper _fileUploadHelper;
    private readonly MediaFileRepository _mediaFileRepository;

    public MediaService(FileUploadHelper fileUploadHelper, MediaFileRepository mediaFileRepository)
    {
        _fileUploadHelper = fileUploadHelper;
        _mediaFileRepository = mediaFileRepository;
    }

    public async Task<DTOs.MediaFileResponse> UploadImageAsync(IFormFile file, string? uploadedBy, CancellationToken cancellationToken = default)
    {
        _fileUploadHelper.ValidateImage(file);
        var url = await _fileUploadHelper.SaveFileAsync(file, "images", cancellationToken);
        return await CreateMediaRecordAsync(file, "image", url, uploadedBy, cancellationToken);
    }

    public async Task<DTOs.MediaFileResponse> UploadAudioAsync(IFormFile file, string? uploadedBy, CancellationToken cancellationToken = default)
    {
        _fileUploadHelper.ValidateAudio(file);
        var url = await _fileUploadHelper.SaveFileAsync(file, "audio", cancellationToken);
        return await CreateMediaRecordAsync(file, "audio", url, uploadedBy, cancellationToken);
    }

    private async Task<DTOs.MediaFileResponse> CreateMediaRecordAsync(IFormFile file, string fileType, string url, string? uploadedBy, CancellationToken cancellationToken)
    {
        var entity = new MediaFile
        {
            FileName = Path.GetFileName(url),
            OriginalFileName = file.FileName,
            Url = url,
            ContentType = file.ContentType,
            FileType = fileType,
            SizeBytes = file.Length,
            StorageProvider = "local",
            UploadedBy = uploadedBy
        };
        await _mediaFileRepository.CreateAsync(entity, cancellationToken);
        return new DTOs.MediaFileResponse
        {
            Id = entity.Id,
            FileName = entity.FileName,
            OriginalFileName = entity.OriginalFileName,
            Url = entity.Url,
            ContentType = entity.ContentType,
            FileType = entity.FileType,
            SizeBytes = entity.SizeBytes
        };
    }
}
