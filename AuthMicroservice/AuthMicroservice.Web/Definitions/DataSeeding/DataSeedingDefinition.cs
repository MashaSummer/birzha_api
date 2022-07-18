using AuthMicroservice.Infrastructure.DatabaseInitialization;
using AuthMicroservice.Web.Definitions.Base;

namespace AuthMicroservice.Web.Definitions.DataSeeding;

/// <summary>
/// Seeding DbContext for default data for EntityFrameworkCore
/// </summary>
public class DataSeedingDefinition : AppDefinition
{
    /// <summary>
    /// Configure application for current application
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
    {
        DatabaseInitializer.SeedUsers(app.Services);
        //DatabaseInitializer.SeedEvents(app.Services); TODO: Add seed event after adding event infrastructure
    }
}