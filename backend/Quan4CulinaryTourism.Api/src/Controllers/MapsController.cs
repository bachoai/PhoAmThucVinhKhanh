using Microsoft.AspNetCore.Mvc;
using Quan4CulinaryTourism.Api.Common;
using Quan4CulinaryTourism.Api.Services;

namespace Quan4CulinaryTourism.Api.Controllers;

[Route($"{AppConstants.ApiVersionPrefix}/maps")]
public class MapsController : BaseApiController
{
    private readonly MapsService _mapsService;
    public MapsController(MapsService mapsService) => _mapsService = mapsService;

    [HttpGet("pack-manifest")]
    public Task<IActionResult> GetPackManifest() => ExecuteAsync(() => _mapsService.GetPackManifestAsync(), "Lấy map manifest thành công");
}
