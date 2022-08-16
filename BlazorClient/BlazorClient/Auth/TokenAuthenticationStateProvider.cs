using BlazorClient.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

public class TokenAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;

    public TokenAuthenticationStateProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorageService.GetAsync<SecurityToken>(nameof(SecurityToken));

        if (token == null)
        {
            return CreateAnonymous();
        }

        if (string.IsNullOrEmpty(token.AccessToken) || token.ExpiredAt < DateTime.UtcNow)
        {
            return CreateAnonymous();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Country, "Russia"),
            new Claim(ClaimTypes.Name, token.UserName),
            new Claim(ClaimTypes.Expired, token. ExpiredAt. ToLongDateString ()),
            new Claim(ClaimTypes.Role, "Administrator"),
            new Claim(ClaimTypes.Role, "Manager"),
            new Claim("Blazor", "Rulezzz")
        };
        var identity = new ClaimsIdentity(claims, "Token");
        var principal = new ClaimsPrincipal(identity);

        return new AuthenticationState(principal);


    }
    public AuthenticationState CreateAnonymous() 
    {
        var anonymousIdentity = new ClaimsIdentity();
        var anonymousPrincipal = new ClaimsPrincipal(anonymousIdentity);
        return new AuthenticationState(anonymousPrincipal);
    }

    public void MakeUserAnonymous()
    {
        _localStorageService.RemoveAsync(nameof(SecurityToken));

        var anonymousIdentity = new ClaimsIdentity();
        var anonymousPrincipal = new ClaimsPrincipal(anonymousIdentity);
        var authState = Task.FromResult(new AuthenticationState(anonymousPrincipal));
        NotifyAuthenticationStateChanged(authState);
    }
}