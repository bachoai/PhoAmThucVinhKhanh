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
    private readonly PythonTextToSpeechService _pythonTextToSpeechService;
    private readonly ILogger<AudioService> _logger;

    public AudioService(
        PoiRepository poiRepository,
        PoiAudioRepository poiAudioRepository,
        FileUploadHelper fileUploadHelper,
        PythonTextToSpeechService pythonTextToSpeechService,
        ILogger<AudioService> logger)
    {
        _poiRepository = poiRepository;
        _poiAudioRepository = poiAudioRepository;
        _fileUploadHelper = fileUploadHelper;
        _pythonTextToSpeechService = pythonTextToSpeechService;
        _logger = logger;
    }

    public Task<List<AudioLanguageResponse>> GetLanguagesAsync(CancellationToken cancellationToken = default)
    {
        var languages = SharedConstants.SupportedLanguages.Select(lang => new AudioLanguageResponse { Code = lang, Name = lang }).ToList();
        return Task.FromResult(languages);
    }

    public async Task<PoiAudioResponse?> GetPoiAudioAsync(string poiId, string? lang, CancellationToken cancellationToken = default)
    {
        var normalizedLang = string.IsNullOrWhiteSpace(lang) ? "vi" : lang.Trim().ToLowerInvariant();
        PoiAudio? audio = null;
        if (!string.IsNullOrWhiteSpace(normalizedLang))
        {
            audio = await _poiAudioRepository.GetByPoiAndLangAsync(poiId, normalizedLang, cancellationToken);
        }

        if (audio is null && normalizedLang == "vi")
        {
            var generated = await GenerateNarrationAudioAsync(poiId, normalizedLang, cancellationToken);
            if (generated is not null)
            {
                return generated;
            }
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

    private async Task<PoiAudioResponse?> GenerateNarrationAudioAsync(string poiId, string lang, CancellationToken cancellationToken)
    {
        try
        {
            var poi = await _poiRepository.GetByIdAsync(poiId, cancellationToken);
            if (poi is null)
            {
                return null;
            }

            var narrationText = string.IsNullOrWhiteSpace(poi.TtsScript) ? poi.Description : poi.TtsScript;
            if (string.IsNullOrWhiteSpace(narrationText))
            {
                return null;
            }

            var generated = await _pythonTextToSpeechService.GenerateVietnameseAudioAsync(narrationText, cancellationToken);
            if (generated is null)
            {
                return null;
            }

            var audio = new PoiAudio
            {
                PoiId = poi.Id,
                Lang = lang,
                AudioUrl = generated.PublicUrl,
                VoiceName = generated.VoiceName,
                SourceType = "python_tts",
                Status = SharedConstants.AudioDone,
                FileSizeBytes = generated.FileSizeBytes
            };

            await _poiAudioRepository.UpsertAsync(audio, cancellationToken);
            poi.AudioStatus = SharedConstants.AudioDone;
            await _poiRepository.UpdateAsync(poi, cancellationToken);
            return ToResponse(audio);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to auto-generate narration audio for POI {PoiId}", poiId);
            return null;
        }
    }
}
