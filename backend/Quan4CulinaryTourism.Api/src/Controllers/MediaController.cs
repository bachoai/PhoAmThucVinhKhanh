using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quan4CulinaryTourism.Api.Common;
using Quan4CulinaryTourism.Api.Helpers;
using Quan4CulinaryTourism.Api.Models;
using Quan4CulinaryTourism.Api.Services;

namespace Quan4CulinaryTourism.Api.Controllers;

[Authorize(Roles = SharedConstants.UserRoles.Admin)]
[Route($"{AppConstants.ApiVersionPrefix}/admin/media")]
public class MediaController : BaseApiController
{
    private readonly MediaService _mediaService;
    private readonly ClaimsHelper _claimsHelper;

    public MediaController(MediaService mediaService, ClaimsHelper claimsHelper)
    {
        _mediaService = mediaService;
        _claimsHelper = claimsHelper;
    }

    [HttpPost("upload-image")]
    public Task<IActionResult> UploadImage(IFormFile file) => ExecuteAsync(() => _mediaService.UploadImageAsync(file, _claimsHelper.GetUserId(User)), "Upload ảnh thành công");

    [HttpPost("upload-audio")]
    public Task<IActionResult> UploadAudio(IFormFile file) => ExecuteAsync(() => _mediaService.UploadAudioAsync(file, _claimsHelper.GetUserId(User)), "Upload audio thành công");
}
