using Microsoft.Extensions.Options;
using Quan4CulinaryTourism.Api.Common;
using Quan4CulinaryTourism.Api.Database;

namespace Quan4CulinaryTourism.Api.Helpers;

public class FileUploadHelper
{
    private readonly UploadSettings _settings;
    private readonly IWebHostEnvironment _environment;

    public FileUploadHelper(IOptions<UploadSettings> settings, IWebHostEnvironment environment)
    {
        _settings = settings.Value;
        _environment = environment;
    }

    public void ValidateImage(IFormFile file)
    {
        Validate(file, AppConstants.SupportedImageExtensions, AppConstants.SupportedImageMimeTypes, _settings.MaxImageSizeMb);
    }

    public void ValidateAudio(IFormFile file)
    {
        Validate(file, AppConstants.SupportedAudioExtensions, AppConstants.SupportedAudioMimeTypes, _settings.MaxAudioSizeMb);
    }

    public string GenerateSafeFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        return $"{Guid.NewGuid():N}{extension}".ToLowerInvariant();
    }

    public (string FullPath, string PublicUrl) CreateManagedFilePath(string subFolder, string extension)
    {
        var normalizedExtension = extension.StartsWith('.') ? extension : $".{extension}";
        var safeFileName = $"{Guid.NewGuid():N}{normalizedExtension}".ToLowerInvariant();
        var rootPath = Path.Combine(_environment.ContentRootPath, _settings.UploadPath, subFolder);
        Directory.CreateDirectory(rootPath);
        var fullPath = Path.Combine(rootPath, safeFileName);
        return (fullPath, $"/uploads/{subFolder}/{safeFileName}");
    }

    public async Task<string> SaveFileAsync(IFormFile file, string subFolder, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.FileName);
        var (fullPath, publicUrl) = CreateManagedFilePath(subFolder, extension);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        return publicUrl;
    }

    private static void Validate(
        IFormFile file,
        IReadOnlyCollection<string> allowedExtensions,
        IReadOnlyCollection<string> allowedMimeTypes,
        int maxSizeMb)
    {
        if (file.Length <= 0)
        {
            throw new ApiException("File tải lên đang rỗng.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            throw new ApiException("Định dạng file không hợp lệ.");
        }

        if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            throw new ApiException("MIME type không hợp lệ.");
        }

        var maxBytes = maxSizeMb * 1024L * 1024L;
        if (file.Length > maxBytes)
        {
            throw new ApiException($"File vượt quá giới hạn {maxSizeMb}MB.");
        }
    }
}
