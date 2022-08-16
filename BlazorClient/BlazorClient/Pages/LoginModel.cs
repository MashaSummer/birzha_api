using BlazorClient.Infrastructure;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Pages
{
    public class LoginModel : ComponentBase
    {
        [Inject] public ILocalStorageService localStorageService { get; set; }

        [Inject] public NavigationManager NavigationManager { get; set; }
        public LoginModel()
        {
            LoginData = new LoginViewModel();
        }

        public LoginViewModel LoginData { get; set; }
        protected async Task LoginAsync()
        {
            var token = new SecurityToken
            {
                AccessToken = LoginData.Password,
                UserName = LoginData.UserName,
                ExpiredAt = DateTime.UtcNow.AddDays(1)
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
