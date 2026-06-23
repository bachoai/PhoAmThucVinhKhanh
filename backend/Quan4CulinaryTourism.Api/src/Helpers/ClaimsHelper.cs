using System.Security.Claims;
using Quan4CulinaryTourism.Api.Common;

namespace Quan4CulinaryTourism.Api.Helpers;

public class ClaimsHelper
{
    public string GetUserId(ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new ApiException("Không tìm thấy userId trong token.", StatusCodes.Status401Unauthorized);
    }

    public string? GetEmail(ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.Email);
    public IReadOnlyCollection<string> GetRoles(ClaimsPrincipal user) => user.FindAll(ClaimTypes.Role).Select(static claim => claim.Value).ToList();
}
