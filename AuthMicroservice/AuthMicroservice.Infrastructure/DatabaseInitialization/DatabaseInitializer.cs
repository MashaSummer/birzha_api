using AuthMicroservice.Domain;
using AuthMicroservice.Domain.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AuthMicroservice.Infrastructure.DatabaseInitialization;

/// <summary>
/// Database Initializer
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Seeds one default users to database for demo purposes only
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static async void SeedUsers(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        // ATTENTION!
        // -----------------------------------------------------------------------------
        // It should be uncomment when using UseSqlServer() settings or any other providers.
        // This is should not be used when UseInMemoryDatabase()
        // await context!.Database.MigrateAsync();
        // -----------------------------------------------------------------------------

        var roles = AppData.Roles.ToArray();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var t11 = await userManager.FindByNameAsync("asd");
        var t = await roleManager.FindByNameAsync("asd");
        foreach (var role in roles)
        {
            
            if (true)
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = role, NormalizedName = role.ToUpper() });
            }
        }

        #region developer

        var developer1 = new ApplicationUser
        {
            Email = "microservice@yopmail.com",
            NormalizedEmail = "MICROSERVICE@YOPMAIL.COM",
            UserName = "microservice@yopmail.com",
            FirstName = "Microservice",
            LastName = "Administrator",
            NormalizedUserName = "MICROSERVICE@YOPMAIL.COM",
            PhoneNumber = "+79000000000",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D"),
            ApplicationUserProfile = new ApplicationUserProfile
            {
                CreatedAt = DateTime.Now,
                CreatedBy = "SEED",
                Permissions = new List<AppPermission>
                {
                    new()
                    {
                        CreatedAt = DateTime.Now,
                        CreatedBy = "SEED",
                        PolicyName = "EventItems:UserRoles:View",
                        Description = "Access policy for EventItems controller user view"
                    }
                }
            }
        };
        var admin = await userManager.FindByEmailAsync(developer1.Email);
        if (admin == null)
        {
            var result = await userManager.CreateAsync(developer1, "123qwe!@#");

            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Cannot create account");
            }

            foreach (var role in roles)
            {
                var roleAdded = await userManager!.AddToRoleAsync(developer1, role);
            }
        }
        /*
        if (!context!.Users.Any(u => u.UserName == developer1.UserName))
        {
            var password = new PasswordHasher<ApplicationUser>();
            var hashed = password.HashPassword(developer1, "123qwe!@#");
            developer1.PasswordHash = hashed;
            var userStore = scope.ServiceProvider.GetRequiredService<ApplicationUserStore>();
            var result = await userStore.CreateAsync(developer1);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Cannot create account");
            }

            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            foreach (var role in roles)
            {
                var roleAdded = await userManager!.AddToRoleAsync(developer1, role);
                if (roleAdded.Succeeded)
                {
                    await context.SaveChangesAsync();
                }
            }
        }
        */

        #endregion
        // loging from template TODO: edit to mongoDb saving log
        /*
        await context.EventItems.AddAsync(new EventItem
        {
            CreatedAt = DateTime.UtcNow,
            Id = Guid.Parse("1b830921-dfab-4093-4b97-99df0136c55f"),
            Level = "Information",
            Logger = "SEED",
            Message = "Seed method some entities successfully save to ApplicationDbContext"
        });

        await context.SaveChangesAsync();
        */
    }

    /// <summary>
    /// Seeds one event to database for demo purposes only
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static async void SeedEvents(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        await using var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

        // It should be uncomment when using UseSqlServer() settings or any other providers.
        // This is should not be used when UseInMemoryDatabase()
        // await context!.Database.MigrateAsync();

        await context!.EventItems.AddAsync(new EventItem
        {
            CreatedAt = DateTime.UtcNow,
            Id = Guid.Parse("1467a5b9-e61f-82b0-425b-7ec75f5c5029"),
            Level = "Information",
            Logger = "SEED",
            Message = "Seed method some entities successfully save to ApplicationDbContext"
        });

        await context.SaveChangesAsync();
    }


}