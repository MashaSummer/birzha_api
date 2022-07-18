using Microsoft.AspNetCore.Identity;

namespace AuthMicroservice.Infrastructure;

/// <summary>
/// Application role
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
}