using Microsoft.AspNetCore.Http;
using Quan4CulinaryTourism.Api.Common;
using Quan4CulinaryTourism.Api.DTOs;
using Quan4CulinaryTourism.Api.Helpers;
using Quan4CulinaryTourism.Api.Models;
using Quan4CulinaryTourism.Api.Repositories;

namespace Quan4CulinaryTourism.Api.Services;

public class AudioService
{
    private readonly PoiRepository _poiRepository;
    private readonly PoiAudioRepository _poiAudioRepository;
    private readonly FileUploadHelper _fileUploadHelper;

    public AudioService(PoiRepository poiRepository, PoiAudioRepository poiAudioRepository, FileUploadHelper fileUploadHelper)
    {
        _poiRepository = poiRepository;
        _poiAudioRepository = poiAudioRepository;
        _fileUploadHelper = fileUploadHelper;
    }

    public Task<List<AudioLanguageResponse>> GetLanguagesAsync(CancellationToken cancellationToken = default)
    {
        var languages = SharedConstants.SupportedLanguages.Select(lang => new AudioLanguageResponse { Code = lang, Name = lang }).ToList();
        return Task.FromResult(languages);
    }

    public async Task<PoiAudioResponse?> GetPoiAudioAsync(string poiId, string? lang, CancellationToken cancellationToken = default)
    {
        PoiAudio? audio = null;
        if (!string.IsNullOrWhiteSpace(lang))
        {
            audio = await _poiAudioRepository.GetByPoiAndLangAsync(poiId, lang, cancellationToken);
        }

        audio ??= (await _poiAudioRepository.GetByPoiIdAsync(poiId, cancellationToken)).FirstOrDefault();
        return audio is null ? null : ToResponse(audio);
    }

    public async Task<PoiAudioResponse> UploadOrSetAudioAsync(string poiId, UploadPoiAudioRequest request, IFormFile? file, CancellationToken cancellationToken = default)
    {
        var poi = await _poiRepository.GetByIdAsync(poiId, cancellationToken)
            ?? throw new ApiException("Không tìm thấy POI.", StatusCodes.Status404NotFound);

        string audioUrl;
        long fileSize = 0;
        if (file is not null)
        {
            _fileUploadHelper.ValidateAudio(file);
            audioUrl = await _fileUploadHelper.SaveFileAsync(file, "audio", cancellationToken);
            fileSize = file.Length;
        }
        else if (!string.IsNullOrWhiteSpace(request.AudioUrl))
        {
            audioUrl = request.AudioUrl;
        }
        else
        {
            throw new ApiException("Cần upload file hoặc truyền AudioUrl.");
        }

        var audio = new PoiAudio
        {
            PoiId = poi.Id,
            Lang = request.Lang,
            AudioUrl = audioUrl,
            VoiceName = request.VoiceName,
            SourceType = request.SourceType,
            Status = SharedConstants.AudioDone,
            FileSizeBytes = fileSize
        };

        await _poiAudioRepository.UpsertAsync(audio, cancellationToken);
        poi.AudioStatus = SharedConstants.AudioDone;
        await _poiRepository.UpdateAsync(poi, cancellationToken);
        return ToResponse(audio);
    }

    public async Task<object> GetPackManifestAsync(CancellationToken cancellationToken = default)
    {
        var pois = await _poiRepository.GetPublicPoisAsync(cancellationToken);
        var items = new List<object>();
        foreach (var poi in pois)
        {
            var audios = await _poiAudioRepository.GetByPoiIdAsync(poi.Id, cancellationToken);
            items.Add(new
            {
                poiId = poi.Id,
                poiName = poi.Name,
                audios = audios.Select(a => new { a.Lang, a.AudioUrl, a.Status })
            });
        }

        return new
        {
            version = "v1",
            generatedAt = DateTime.UtcNow,
            items
        };
    }

    private static PoiAudioResponse ToResponse(PoiAudio audio) => new()
    {
        Id = audio.Id,
        PoiId = audio.PoiId,
        Lang = audio.Lang,
        AudioUrl = audio.AudioUrl,
        VoiceName = audio.VoiceName,
        SourceType = audio.SourceType,
        Status = audio.Status,
        DurationSeconds = audio.DurationSeconds,
        FileSizeBytes = audio.FileSizeBytes
    };
}
