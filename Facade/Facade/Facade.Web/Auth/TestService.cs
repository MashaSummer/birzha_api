using AuthRequest;
using Grpc.Core;

namespace Facade.Web.Auth;

public class TestService : AuthService.AuthServiceBase
{
    private readonly ILogger<TestService> _logger;

    public TestService(ILogger<TestService> logger) => _logger = logger;

    public override Task<TokenData> Login(LoginData request, ServerCallContext context)
    {
        _logger.Log(LogLevel.Information, $"Login: {request.Email} Password: {request.Password}");

        return Task.FromResult(new TokenData() { Token = "To123456789ken" });
    }
}