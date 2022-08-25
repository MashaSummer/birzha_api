using BlazorClient.Attributes;
using BlazorClient.Infrastructure;
using Blazored.Toast.Services;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using ProductGrpc;
using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Components
{
    public class AddProductModel : ComponentBase
    {
        public Guid Guid = Guid.NewGuid();
        public string ModalDisplay = "none;";
        public string ModalClass = "fade";
        public bool ShowBackdrop = false;
        public AddProductViewModel addProductViewModel = new();
        [Inject] public ILocalStorageService localStorageService { get; set; }
        [Inject] ProductService.ProductServiceClient Client { get; set; }
        [Inject] PriceDefineService priceDefineService { get; set; }
        [Inject] IToastService toastService { get; set; }

        public async Task HandleValidSubmitAsync()
        {
            var t = addProductViewModel;   
            var token = await localStorageService.GetAsync<SecurityToken>(nameof(SecurityToken));
            ChangePortfolioResponse addProductResponse = new();
            try
            {
                var headers = new Metadata();
                headers.Add("Authorization", $"Bearer {token.AccessToken}");
                var changePortfolioRequest = new ChangePortfolioRequest()
                {
                    InvestorId = "",
                    ProductName = addProductViewModel.ProductName,
                    StartPrice = priceDefineService.DefinePrice(addProductViewModel.StartPrice),
                    Volume = addProductViewModel.Volume
                };

                addProductResponse = await Client.AddProductAsync(changePortfolioRequest, headers);

                if (addProductResponse == null || addProductResponse.Error != null)
                {
                    Close();
                    toastService.ShowError($"Unable to add {addProductViewModel.ProductName}");
                    return;
                }
            }
            catch (Exception ex)
            {
                Close();
                toastService.ShowError($"Unable to add {addProductViewModel.ProductName}.");
                return;
            }


            Close();
            toastService.ShowSuccess($"Product {addProductViewModel.ProductName} succesfully added.", "SUCCESS");
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
