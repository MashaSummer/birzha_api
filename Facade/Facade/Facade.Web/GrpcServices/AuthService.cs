using AuthRequest;
using Grpc.Core;

namespace Facade.Web.GrpcServices;

public class AuthService : AuthRequest.AuthService.AuthServiceBase
{
    private readonly ILogger<AuthService> _logger;
    private readonly string _authUrl;

    private static readonly HttpClient _client = new HttpClient();

    public AuthService(ILogger<AuthService> logger, string authUrl)
    {
        _logger = logger;
        _authUrl = authUrl;
    }

    
    /// <summary>
    /// Try to get auth token for user
    /// </summary>
    /// <param name="request">user email and password</param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TokenData> Login(LoginData request, ServerCallContext context)
    {
        _logger.Log(LogLevel.Information, $"Got login request");

        var response = "No response";
        var hasError = false;
        var errorText = "No error";

        try
        {
            response = await _client.GetStringAsync(_authUrl);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error on login method: {e.Message}");

            hasError = true;
            errorText = e.Message;
        }

        return await Task.FromResult(new TokenData() { Token = response, ErrorText = errorText, HasError = hasError });
    }
}