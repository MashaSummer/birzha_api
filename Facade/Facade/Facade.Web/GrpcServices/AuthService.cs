using AuthRequest;
using Grpc.Core;

namespace Facade.Web.GrpcServices;

public class AuthService : AuthRequest.AuthService.AuthServiceBase
{
    private readonly ILogger<AuthService> _logger;
    private readonly string _authUrl;

    private readonly HttpClient _client;

    public AuthService(ILogger<AuthService> logger, string authUrl, HttpClient client)
    {
        _logger = logger;
        _authUrl = authUrl;
        _client = client;
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
            var authResponse = await _client.SendAsync(CreateMessage(request.Email, request.Password));
            authResponse.EnsureSuccessStatusCode();
            response = await authResponse.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            _logger.LogError($"Error on login method: {e.Message}");

            hasError = true;
            errorText = e.Message;
        }

        return await Task.FromResult(new TokenData() { Token = response, ErrorText = errorText, HasError = hasError });
    }


    private HttpRequestMessage CreateMessage(string email, string password)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, _authUrl);
        
        var messageDict = new Dictionary<string, string>()
        {
            { "client_id", "service-to-service" },
            { "client_secret", "client_secret_sts" },
            { "scope", "api" },
            { "grant_type", "password" },
            { "username", email },
            { "password", password }
        };
        
        message.Content = new FormUrlEncodedContent(messageDict);

        return message;
    }
}