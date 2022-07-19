using AuthRequest;
using Grpc.Core;

namespace Facade.Web.Auth;

public class AuthService : AuthRequest.AuthService.AuthServiceBase
{
    private readonly ILogger<AuthService> _logger;

    public AuthService(ILogger<AuthService> logger) => _logger = logger;

    
    public override Task<TokenData> Login(LoginData request, ServerCallContext context)
    {
        _logger.LogInformation($"Got login request: {request}");
        return Task.FromResult(new TokenData() { Token = "To12345678ken" });
    }
}