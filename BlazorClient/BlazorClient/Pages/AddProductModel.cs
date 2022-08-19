using Microsoft.AspNetCore.Components;

namespace BlazorClient.Pages
{
    public class AddProductModel : ComponentBase
    {
        public Guid Guid = Guid.NewGuid();
        public string ModalDisplay = "none;";
        public string ModalClass = "";
        public bool ShowBackdrop = false;

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
}
