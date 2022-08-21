using BalanceMicroservice;
using BlazorClient.Infrastructure;
using Blazored.Toast.Services;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Pages
{
    public class BalanceModel : ComponentBase
    {
        [Inject] IConfiguration config { get; set; }
        [Inject] PriceDefineService priceDefineService { get; set; }
        [Inject] IToastService toastService { get; set; }
        public Components.AddProductModal Modal { get; set; }
        public double Balance { get; set; }

        protected override async Task OnInitializedAsync()
        {

            var address = config["FacadeBaseURL"];
            BalanceResponse balanceResponse = new();
            try
            {
                var channel = GrpcChannel.ForAddress(address);

                var client = new QueryBalanceService.QueryBalanceServiceClient(channel);


                balanceResponse = await client.GetBalanceAsync(new GetBalanceRequest { Id = null });


                if (balanceResponse == null || balanceResponse.Error != null)
                {

                    toastService.ShowError($"Enable to fetch portfolio, please try again.");
                    return;
                }
                Balance = balanceResponse.Balance;
            }
            catch (Exception ex)
            {
                toastService.ShowError($"Enable to fetch products, please try again");
                return;
            }
        }
    }
}