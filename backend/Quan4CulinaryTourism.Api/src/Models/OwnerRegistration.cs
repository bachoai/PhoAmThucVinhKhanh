namespace Quan4CulinaryTourism.Api.Models;

public class OwnerRegistration : BaseDocument
{
    public string UserId { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = SharedConstants.OwnerPending;
    public string? AdminNote { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
