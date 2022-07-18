using AuthRequest;
using Grpc.Core;

namespace Facade.Web.Definitions.AuthDefinition;

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
        var response = "None";
        var hasError = false;
        var errorText = "None";

        try
        {
            response = await _client.GetStringAsync(_authUrl);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            hasError = true;
            errorText = e.Message;
        }

        Console.WriteLine($"Sending message: {response} {hasError} {errorText}");

        return await Task.FromResult(new TokenData() { Token = response, ErrorText = errorText, HasError = hasError });
    }
}