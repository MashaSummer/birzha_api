using Balances;
using BlazorClient.Infrastructure;
using Blazored.Toast.Services;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Pages
{
    public class BalanceModel : ComponentBase
    {
        [Inject] IConfiguration config { get; set; }
        [Inject] IToastService toastService { get; set; }
        public Components.AddProductModal Modal { get; set; }
        public double Balance { get; set; }

        protected override async Task OnInitializedAsync()
        {

            var address = config["FacadeBaseURL"];
            BalanceData balanceResponse = new();
            try
            {
                var channel = GrpcChannel.ForAddress(address);

                var client = new Balance.BalanceClient(channel);


                balanceResponse = await client.GetBalanceAsync(new EmptyRequest());


                if (balanceResponse == null || balanceResponse.Status == BalanceData.Types.Status.Failed)
                {

                    toastService.ShowError($"Enable to fetch balance, please try again.");
                    return;
                }
                Balance = balanceResponse.Balance;
            }
            catch (Exception ex)
            {
                toastService.ShowError($"Enable to fetch balance, please try again.");
                return;
            }
        }
    }
}