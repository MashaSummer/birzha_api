using Microsoft.AspNetCore.Components;

namespace BlazorClient.Pages
{
    public class CatalogModel : ComponentBase
    {
        public Components.AddProductModal Modal { get; set; }

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }
    }
}
