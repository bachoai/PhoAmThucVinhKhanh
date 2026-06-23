using Microsoft.AspNetCore.Http;
using Quan4CulinaryTourism.Api.Common;
using Quan4CulinaryTourism.Api.DTOs;
using Quan4CulinaryTourism.Api.Helpers;
using Quan4CulinaryTourism.Api.Models;
using Quan4CulinaryTourism.Api.Repositories;

namespace Quan4CulinaryTourism.Api.Services;

public class AuthService
{
    private readonly UserRepository _userRepository;
    private readonly OwnerRegistrationRepository _ownerRegistrationRepository;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtHelper _jwtHelper;

    public AuthService(UserRepository userRepository, OwnerRegistrationRepository ownerRegistrationRepository, PasswordHasher passwordHasher, JwtHelper jwtHelper)
    {
        _userRepository = userRepository;
        _ownerRegistrationRepository = ownerRegistrationRepository;
        _passwordHasher = passwordHasher;
        _jwtHelper = jwtHelper;
    }

    public async Task<CurrentUserResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
        {
            throw new ApiException("Email đã tồn tại.");
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber,
            Roles = [SharedConstants.UserRoles.User],
            OwnerStatus = SharedConstants.OwnerPending
        };

        await _userRepository.CreateAsync(user, cancellationToken);
        return ToCurrentUserResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new ApiException("Email hoặc mật khẩu không đúng.", StatusCodes.Status401Unauthorized);

        if (!user.IsActive)
        {
            throw new ApiException("Tài khoản đã bị khóa.", StatusCodes.Status403Forbidden);
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new ApiException("Email hoặc mật khẩu không đúng.", StatusCodes.Status401Unauthorized);
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new AuthResponse
        {
            Token = _jwtHelper.GenerateToken(user),
            User = ToCurrentUserResponse(user)
        };
    }

    public async Task<CurrentUserResponse> GetCurrentUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new ApiException("Không tìm thấy người dùng.", StatusCodes.Status404NotFound);
        return ToCurrentUserResponse(user);
    }

    public async Task<OwnerRegistrationResponse> RegisterOwnerAsync(string userId, CreateOwnerRegistrationRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new ApiException("Không tìm thấy người dùng.", StatusCodes.Status404NotFound);

        var registration = new OwnerRegistration
        {
            UserId = user.Id,
            BusinessName = request.BusinessName,
            BusinessAddress = request.BusinessAddress,
            PhoneNumber = request.PhoneNumber,
            Description = request.Description,
            Status = SharedConstants.OwnerPending
        };

        await _ownerRegistrationRepository.CreateAsync(registration, cancellationToken);
        return new OwnerRegistrationResponse
        {
            Id = registration.Id,
            UserId = registration.UserId,
            BusinessName = registration.BusinessName,
            BusinessAddress = registration.BusinessAddress,
            PhoneNumber = registration.PhoneNumber,
            Description = registration.Description,
            Status = registration.Status,
            AdminNote = registration.AdminNote,
            CreatedAt = registration.CreatedAt
        };
    }

    public static CurrentUserResponse ToCurrentUserResponse(User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        AvatarUrl = user.AvatarUrl,
        Roles = user.Roles,
        IsActive = user.IsActive,
        EmailVerified = user.EmailVerified,
        OwnerStatus = user.OwnerStatus
    };
}
