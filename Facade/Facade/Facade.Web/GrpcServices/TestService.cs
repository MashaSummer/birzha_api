using AuthRequest;
using Grpc.Core;

namespace Facade.Web.GrpcServices;

public class TestService : AuthService.AuthServiceBase
{
    private readonly ILogger<TestService> _logger;

    public TestService(ILogger<TestService> logger) => _logger = logger;

    public override Task<TokenData> Login(LoginData request, ServerCallContext context)
    {
        _logger.Log(LogLevel.Information, $"Login: {request.Email1} Password: {request.Password}");

        return Task.FromResult(new TokenData() { Token1 = "To123456789ken" });
    }
}