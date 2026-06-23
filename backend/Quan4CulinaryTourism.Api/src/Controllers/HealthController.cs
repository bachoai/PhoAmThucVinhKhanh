using Microsoft.AspNetCore.Mvc;
using Quan4CulinaryTourism.Api.Services;

namespace Quan4CulinaryTourism.Api.Controllers;

[Route("api/health")]
public class HealthController : BaseApiController
{
    private readonly HealthService _healthService;
    public HealthController(HealthService healthService) => _healthService = healthService;

    [HttpGet]
    public Task<IActionResult> Check() => ExecuteAsync(() => _healthService.CheckAsync(), "Health check thành công");
}
