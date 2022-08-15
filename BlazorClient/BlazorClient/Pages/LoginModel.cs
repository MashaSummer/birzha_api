using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Pages
{
    public class LoginModel : ComponentBase
    {
    }

    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
