using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quan4CulinaryTourism.Api.Common;
using Quan4CulinaryTourism.Api.DTOs;
using Quan4CulinaryTourism.Api.Helpers;
using Quan4CulinaryTourism.Api.Services;

namespace Quan4CulinaryTourism.Api.Controllers;

[Route($"{AppConstants.ApiVersionPrefix}/auth")]
public class AuthController : BaseApiController
{
    private readonly AuthService _authService;
    private readonly ClaimsHelper _claimsHelper;

    public AuthController(AuthService authService, ClaimsHelper claimsHelper)
    {
        _authService = authService;
        _claimsHelper = claimsHelper;
    }

    [HttpPost("register")]
    public Task<IActionResult> Register([FromBody] RegisterRequest request) => ExecuteAsync(() => _authService.RegisterAsync(request), "Đăng ký thành công");

    [HttpPost("login")]
    public Task<IActionResult> Login([FromBody] LoginRequest request) => ExecuteAsync(() => _authService.LoginAsync(request), "Đăng nhập thành công");

    [Authorize]
    [HttpGet("me")]
    public Task<IActionResult> Me() => ExecuteAsync(() => _authService.GetCurrentUserAsync(_claimsHelper.GetUserId(User)), "Lấy thông tin thành công");

    [Authorize]
    [HttpPost("register-owner")]
    public Task<IActionResult> RegisterOwner([FromBody] CreateOwnerRegistrationRequest request) =>
        ExecuteAsync(() => _authService.RegisterOwnerAsync(_claimsHelper.GetUserId(User), request), "Gửi yêu cầu owner thành công");
}
