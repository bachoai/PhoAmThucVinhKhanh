using Quan4CulinaryTourism.Api.DTOs;
using Quan4CulinaryTourism.Api.Repositories;

namespace Quan4CulinaryTourism.Api.Services;

public class MapsService
{
    private readonly MapPackRepository _mapPackRepository;

    public MapsService(MapPackRepository mapPackRepository)
    {
        _mapPackRepository = mapPackRepository;
    }

    public async Task<MapPackResponse?> GetPackManifestAsync(CancellationToken cancellationToken = default)
    {
        var pack = await _mapPackRepository.GetActiveAsync(cancellationToken);
        if (pack is null)
        {
            return null;
        }

        return new MapPackResponse
        {
            Id = pack.Id,
            Version = pack.Version,
            Name = pack.Name,
            DownloadUrl = pack.DownloadUrl,
            Sha256 = pack.Sha256,
            SizeBytes = pack.SizeBytes,
            IsActive = pack.IsActive,
            PublishedAt = pack.PublishedAt
        };
    }
}
