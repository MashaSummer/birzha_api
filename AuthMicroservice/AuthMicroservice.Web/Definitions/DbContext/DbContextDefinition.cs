using AuthMicroservice.Infrastructure;
using AuthMicroservice.Web.Definitions.Base;
using AuthMicroservice.Web.Definitions.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using System.Security.Claims;

namespace AuthMicroservice.Web.Definitions.DbContext;

/// <summary>
/// ASP.NET Core services registration and configurations
/// </summary>
public class DbContextDefinition : AppDefinition
{
    /// <summary>
    /// Configure services for current application
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(config =>
        {
            // UseInMemoryDatabase - This for demo purposes only!
            // Should uninstall package "Microsoft.EntityFrameworkCore.InMemory" and install what you need. 
            // For example: "Microsoft.EntityFrameworkCore.SqlServer"
            // uncomment line below to use UseSqlServer(). Don't forget setup connection string in appSettings.json 
            config.UseInMemoryDatabase("DEMO-PURPOSES-ONLY");

            // Register the entity sets needed by OpenIddict.
            // Note: use the generic overload if you need to replace the default OpenIddict entities.
            config.UseOpenIddict<Guid>();
        });


        

        

        services.AddTransient<ApplicationUserStore>();
    }
}
