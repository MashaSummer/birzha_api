using Balances;
using BlazorClient.Infrastructure;
using Blazored.Toast.Services;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Pages
{
    public class BalanceModel : ComponentBase
    {
        [Inject] IToastService toastService { get; set; }
        [Inject] public ILocalStorageService localStorageService { get; set; }
        [Inject] Balance.BalanceClient Client { get; set; }
        public Components.DepositModal Modal { get; set; }
        public double Balance { get; set; }
        public double Frozen { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var token = await localStorageService.GetAsync<SecurityToken>(nameof(SecurityToken));
            BalanceData balanceResponse = new();
            try
            {
                var headers = new Metadata();
                headers.Add("Authorization", $"Bearer {token}");

                balanceResponse = await Client.GetBalanceAsync(new EmptyRequest(), headers);

                if (balanceResponse == null || balanceResponse.Status == BalanceData.Types.Status.Failed)
                {
                    toastService.ShowError($"Unable to fetch balance, please try again.");
                    return;
                }
                Balance = balanceResponse.Balance;
                Frozen = balanceResponse.FrozenBalance;
            }
            catch (Exception ex)
            {
                toastService.ShowError($"Unable to fetch balance, please try again.");
                return;
            }
        }
    }
}