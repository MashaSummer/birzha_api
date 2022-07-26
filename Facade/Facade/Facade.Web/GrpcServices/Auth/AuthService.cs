using AuthRequest;
using Calabonga.OperationResults;
using Facade.Web.GrpcServices.Auth.ViewModels;
using Grpc.Core;
using System.Text.Json;

namespace Facade.Web.Auth;


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
    /// <returns>Object with token</returns>
    public override async Task<TokenData> Login(LoginData request, ServerCallContext context)
    {
        _logger.LogInformation("Got login request");

        var responseResult = await GetToken(request.Email, request.Password);

        
        var toReturn = new TokenData()
        {
            Token = responseResult.Ok ? responseResult.Result : "No response", 
            Status = responseResult.Ok ? TokenData.Types.Status.Success : TokenData.Types.Status.Failed
        };
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


    private async Task<OperationResult<string>> GetToken(string email, string password)
    {
        var result = new OperationResult<string>();

        try
        {
            var authResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress) { Content = CreateMessage(email, password) });
            var tokenResponse = await authResponse.Content.ReadFromJsonAsync<TokenResponseViewModel>();

            if (tokenResponse == null)
            {
                result.AddError("Invalid auth response");
            }
            else
            {
                result.Result = tokenResponse.AccessToken;
            }
        }

        catch (HttpRequestException ex)
        {
            result.AddError(ex.Message);
        }

        catch (JsonException ex)
        {
            result.AddError("Invalid auth response");
        }

        return result;
    }
}