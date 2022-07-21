using AuthRequest;
using Facade.Web.GrpcServices.ViewModels;
using Grpc.Core;

namespace Facade.Web.GrpcServices;

public class AuthService : AuthRequest.AuthService.AuthServiceBase
{
    private readonly ILogger<AuthService> _logger;

    private readonly HttpClient _client;

    public AuthService(ILogger<AuthService> logger, HttpClient client)
    {
        _logger = logger;
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
        _logger.LogInformation("Got login request");

        var response = "No response";
        var hasError = false;

        try
        {
            var authResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress)
            {
                Content = CreateMessage(request.Email, request.Password)
            });
            authResponse.EnsureSuccessStatusCode();
            var tokenResponse = await authResponse.Content.ReadFromJsonAsync<TokenResponseViewModel>();

            if (tokenResponse == null)
            {
                throw new Exception("Invalid auth response");
            }

            response = tokenResponse?.AccessToken;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error on login method: {e.Message}");

            hasError = true;
        }

        _logger.LogInformation($"Has login error: {hasError}");
        var toReturn = new TokenData() { Token = response, Status = hasError ? TokenData.Types.Status.Failed : TokenData.Types.Status.Success };
        return await Task.FromResult(toReturn);
    }


    private FormUrlEncodedContent CreateMessage(string email, string password)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress);
        
        var messageDict = new Dictionary<string, string>()
        {
            { "client_id", "service-to-service" },
            { "client_secret", "client_secret_sts" },
            { "scope", "api" },
            { "grant_type", "password" },
            { "username", email },
            { "password", password }
        };
        
        return new FormUrlEncodedContent(messageDict);
    }
}