using AuthMicroservice.Infrastructure;
using AuthMicroservice.Web.Definitions.Base;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization;
using OpenIddict.Abstractions;

namespace AuthMicroservice.Web.Definitions.MongoIdentity
{
    public class MongoIdentityDefinition : AppDefinition
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:MongoDbConnection"];
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = false;
            })
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
                (
                    connectionString,
                    "openid-identity-test"
                )
                .AddDefaultTokenProviders();

            BsonSerializer.RegisterSerializer(new DictionaryCultureInfoSerializer());

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
                options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
                // configure more options if you need
            });
        }
    }
}
