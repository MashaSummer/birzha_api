using Balances;
using BlazorClient.Attributes;
using BlazorClient.Infrastructure;
using Blazored.Toast.Services;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Components
{
    public class DepositModel : ComponentBase
    {
        public Guid Guid = Guid.NewGuid();
        public string ModalDisplay = "none;";
        public string ModalClass = "fade";
        public bool ShowBackdrop = false;
        public AddBalanceViewModel addBalanceViewModel = new();
        [Inject] public ILocalStorageService localStorageService { get; set; }
        [Inject] Balance.BalanceClient Client { get; set; }
        [Inject] IToastService toastService { get; set; }

        public async Task HandleValidSubmitAsync()
        {
            var token = await localStorageService.GetAsync<SecurityToken>(nameof(SecurityToken));
            BalanceData balanceData= new();
            try
            {
                var headers = new Metadata();
                headers.Add("Authorization", $"Bearer {token.AccessToken}");
                balanceData = await Client.AddBalanceAsync(new ValueRequest() { Value = addBalanceViewModel.Value}, headers);

                if (balanceData == null || balanceData.Status == BalanceData.Types.Status.Failed)
                {
                    Close();
                    toastService.ShowError($"Unable to deposite balance.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Close();
                toastService.ShowError($"Unable to deposite balance.");
                return;
            }

            Close();
            toastService.ShowSuccess($"{addBalanceViewModel.Value} y.e. succesfully added.", "SUCCESS");
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
    public class AddBalanceViewModel
    {
        [Required]
        [BasePrice(ErrorMessage = "Invalid format, example 123.45")]
        public double Value { get; set; }
    }
}

