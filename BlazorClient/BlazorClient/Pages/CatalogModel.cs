using BlazorClient.Infrastructure;
using Blazored.Toast.Services;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using ProductGrpc;

namespace BlazorClient.Pages
{
    public class CatalogModel : ComponentBase
    {
        [Inject] public ILocalStorageService localStorageService { get; set; }
        [Inject] ProductService.ProductServiceClient Client { get; set; }
        [Inject] PriceDefineService priceDefineService { get; set; }
        [Inject] IToastService toastService { get; set; }
        public Components.AddProductModal Modal { get; set; }
        public List<AllProductViewModel> products { get; set; }

        protected override async Task OnInitializedAsync()
        {
            products = new();
            var token = await localStorageService.GetAsync<SecurityToken>(nameof(SecurityToken));
            GetAllProductsResponse getAllProductsResponse = new();
            try
            {
                var headers = new Metadata();
                headers.Add("Authorization", $"Bearer {token.AccessToken}");
                getAllProductsResponse = await Client.GetAllProductsAsync(new GetAllProductsRequest(), headers);

                if (getAllProductsResponse == null || getAllProductsResponse.Error != null)
                {

                    toastService.ShowError($"Enable to fetch products, please try again");
                    return;
                }
                products = getAllProductsResponse.ProductArray.Products.Select(x => new AllProductViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    BestAsk = x.BestAsk,
                    BestBid = x.BestBid
                }).ToList();
            }
            catch (Exception ex)
            {
                toastService.ShowError($"Enable to fetch products, please try again");
                return;
            }
        }
    }
    public class AllProductViewModel
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public double BestAsk { get; set; }
        public double BestBid { get; set; }
    }

}
