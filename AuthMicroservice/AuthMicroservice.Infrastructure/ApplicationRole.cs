using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthMicroservice.Infrastructure;

/// <summary>
/// Application role
/// </summary>
public class ApplicationRole : MongoIdentityRole<Guid>
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }
}