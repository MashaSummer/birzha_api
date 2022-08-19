using BlazorClient.Attributes;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Pages
{
    public class AddProductModel : ComponentBase
    {
        public Guid Guid = Guid.NewGuid();
        public string ModalDisplay = "none;";
        public string ModalClass = "fade";
        public bool ShowBackdrop = false;
        public AddProductViewModel addProductViewModel = new();

        public async Task HandleValidSubmitAsync()
        {
            var t = addProductViewModel;
            // Process the valid form
        }


        public void Open()
        {
            ModalDisplay = "block;";
            ModalClass = "Show";
            ShowBackdrop = true;
            StateHasChanged();
        }

        public void Close()
        {
            ModalDisplay = "none";
            ModalClass = "";
            ShowBackdrop = false;
            StateHasChanged();
        }
    }
    public class AddProductViewModel
    {
        [Required]
        public string ProductName { get; set; }
        [Required]
        public int Volume { get; set; }

        [Required]
        [BasePrice(ErrorMessage ="Invalid format, example 123.45")]
        public double StartPrice { get; set; }
    }
}
