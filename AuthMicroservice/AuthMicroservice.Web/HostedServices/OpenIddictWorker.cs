using AuthMicroservice.Infrastructure;
using OpenIddict.Abstractions;

namespace AuthMicroservice.Web.HostedServices;

public class OpenIddictWorker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public OpenIddictWorker(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        await RegisterApplicationsAsync(scope.ServiceProvider);
    }
    static async Task RegisterApplicationsAsync(IServiceProvider provider, CancellationToken token = default)
    {
        var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();
        var url = provider.GetRequiredService<IConfiguration>().GetValue<string>("AuthServer:Url");
        if (await manager.FindByClientIdAsync("authorization-flow", token) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "authorization-flow",
                ClientSecret = "client_secret_code",
                DisplayName = "API testing clients with Authorization Code Flow demonstration",
                
                PostLogoutRedirectUris =
                        {
                            new Uri("http://localhost:5010/signout-callback-oidc")
                        },
                RedirectUris =
                        {
                            new Uri("https://www.thunderclient.com/oauth/callback"),        // https://www.thunderclient.com/
                            new Uri($"{url}/swagger/oauth2-redirect.html")                  // https://swagger.io/
                        },
                Permissions =
                        {
                            // Endpoint permissions
                            OpenIddictConstants.Permissions.Endpoints.Authorization,
                            OpenIddictConstants.Permissions.Endpoints.Token,

                            // Grant type permissions
                            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,

                            // Scope permissions
                            OpenIddictConstants.Permissions.Prefixes.Scope + "api",
                            OpenIddictConstants.Permissions.Prefixes.Scope + "custom",

                            // Response types
                            OpenIddictConstants.Permissions.ResponseTypes.Code,
                            OpenIddictConstants.Permissions.ResponseTypes.IdToken
                        }
            });
        }
        
        if (await manager.FindByClientIdAsync("service-to-service", token) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "service-to-service",
                ClientSecret = "client_secret_sts",
                DisplayName = "Service-To-Service demonstration",
                Permissions =
                {
                    // Endpoint permissions
                    OpenIddictConstants.Permissions.Endpoints.Token,

                    // Grant type permissions
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    OpenIddictConstants.Permissions.GrantTypes.Password,
                        
                    // Scope permissions
                    OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                }
            }, token);
        }
    }
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}