using AuthRequest;
using Grpc.Core;

namespace Facade.Web.Services;

public class AuthenticationService : Authentication.AuthenticationBase
{
    private static HttpClient _client = new HttpClient();
    private readonly string _authUrl;


    public AuthenticationService(string authUrl) => _authUrl = authUrl;

    
    /// <summary>
    /// Send http request to auth service to get token
    /// </summary>
    /// <param name="request">Object with auth data</param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TokenData> Login(UserData request, ServerCallContext context)
    {
        var response = "";
        var hasError = false;
        var errorText = "";

        try
        {
            response = await _client.GetStringAsync(_authUrl);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);

            hasError = true;
            errorText = e.Message;
        }

        return new TokenData() { Token = response, HasError = hasError, ErrorText = errorText };
    }
}