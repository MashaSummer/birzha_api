using AuthRequest;
using BlazorClient.Infrastructure;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

namespace BlazorClient.Pages
{
    public class LoginModel : ComponentBase
    {
        [Inject] public ILocalStorageService localStorageService { get; set; }

        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] IConfiguration config { get; set; }
        public LoginModel()
        {
            LoginData = new LoginViewModel();
        }

        public LoginViewModel LoginData { get; set; }
        protected async Task LoginAsync()
        {
            var address = config["FacadeBaseURL"];
            TokenData tokenData = new TokenData();
            try
            {
                var channel = GrpcChannel.ForAddress(address);

                var client = new AuthService.AuthServiceClient(channel);
                tokenData = await client.LoginAsync(new LoginData
                {
                    Email = LoginData.UserName,
                    Password = LoginData.Password
                });
                if (tokenData == null || tokenData.Status == TokenData.Types.Status.Failed)
                {
                    NavigationManager.NavigateTo("/loginfailed", true);
                }
            }
            catch (Exception ex)
            {
                NavigationManager.NavigateTo("/loginfailed", true);
            }
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenData?.Token);
            var exp = Convert.ToInt32(jwt.Claims.First(claim => claim.Type == "exp").Value);

            var token = new SecurityToken
            {
                AccessToken = tokenData.Token,
                UserName = LoginData.UserName,
                ExpiredAt = DateTime.UtcNow.AddMilliseconds(exp)
            };

            await localStorageService.SetAsync(nameof(SecurityToken), token);
            NavigationManager.NavigateTo("/", true);
        }
    }

    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
