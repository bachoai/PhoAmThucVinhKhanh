using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quan4CulinaryTourism.Api.Common;
using Quan4CulinaryTourism.Api.DTOs;
using Quan4CulinaryTourism.Api.Models;
using Quan4CulinaryTourism.Api.Services;

namespace Quan4CulinaryTourism.Api.Controllers;

[Route($"{AppConstants.ApiVersionPrefix}/poi")]
public class PoiController : BaseApiController
{
    private readonly PoiService _poiService;
    public PoiController(PoiService poiService) => _poiService = poiService;

    [HttpGet("load-all")]
    public Task<IActionResult> LoadAll([FromQuery] PoiSearchRequest request) => ExecuteAsync(() => _poiService.LoadAllAsync(request), "Lấy POI thành công");

    [HttpGet("{id}")]
    public Task<IActionResult> GetById(string id, [FromQuery] string? lang) => ExecuteAsync(() => _poiService.GetByIdAsync(id, lang), "Lấy chi tiết POI thành công");

    [HttpGet("nearby")]
    public Task<IActionResult> Nearby([FromQuery] double lat, [FromQuery] double lng, [FromQuery] int radius = 3000, [FromQuery] int limit = 20, [FromQuery] string? lang = null)
        => ExecuteAsync(() => _poiService.NearbyAsync(lat, lng, radius, limit, lang), "Lấy POI gần đây thành công");

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] PoiSearchRequest request) => ExecuteAsync(() => _poiService.SearchAsync(request), "Tìm kiếm POI thành công");
}
