using BlazorClient.Infrastructure;
using Blazored.Toast.Services;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using ProductGrpc;

namespace BlazorClient.Pages
{
    public class CatalogModel : ComponentBase
    {
        [Inject] IConfiguration config { get; set; }
        [Inject] PriceDefineService priceDefineService { get; set; }
        [Inject] IToastService toastService { get; set; }
        public Components.AddProductModal Modal { get; set; }
        public IEnumerable<AllProductViewModel> products { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var address = config["FacadeBaseURL"];
            GetAllProductsResponse getAllProductsResponse = new();
            try
            {
                var channel = GrpcChannel.ForAddress(address);

                var client = new ProductService.ProductServiceClient(channel);


                getAllProductsResponse = await client.GetAllProductsAsync(new GetAllProductsRequest());

                
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
                });
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
