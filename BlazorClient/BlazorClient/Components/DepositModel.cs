using Balances;
using BlazorClient.Attributes;
using Blazored.Toast.Services;
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
        [Inject] IConfiguration config { get; set; }
        [Inject] IToastService toastService { get; set; }

        public async Task HandleValidSubmitAsync()
        {
            var address = config["FacadeBaseURL"];
            BalanceData balanceData= new();
            try
            {
                var channel = GrpcChannel.ForAddress(address);

                var client = new Balance.BalanceClient(channel);

                balanceData = await client.AddBalanceAsync(new ValueRequest() { Value = addBalanceViewModel.Value});

                if (balanceData == null || balanceData.Status == BalanceData.Types.Status.Failed)
                {
                    Close();
                    toastService.ShowError($"Unable to deposite balance");
                    return;
                }
            }
            catch (Exception ex)
            {
                Close();
                toastService.ShowError($"Unable to deposite balance");
                return;
            }

            Close();
            toastService.ShowSuccess($"{addBalanceViewModel.Value} y.e succesfully added", "SUCCESS");
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

