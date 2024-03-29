﻿using BlazorClient.Infrastructure;
using Blazored.Toast.Services;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Orders;
using PortfolioServiceGrpc;

namespace BlazorClient.Pages
{
    public class PortfolioModel : ComponentBase
    {
        [Inject] public ILocalStorageService localStorageService { get; set; }
        [Inject] PortfolioService.PortfolioServiceClient Client { get; set; }
        [Inject] PriceDefineService priceDefineService { get; set; }
        [Inject] IToastService toastService { get; set; }
        public Components.AddProductModal Modal { get; set; }
        public PortfolioViewModel portfolio { get; set; }

        protected override async Task OnInitializedAsync()
        {
            portfolio = new();
            var token = await localStorageService.GetAsync<SecurityToken>(nameof(SecurityToken));
            GetPortfolioResponse getPortfolioResponse = new();
            try
            {
                var headers = new Metadata();
                headers.Add("Authorization", $"Bearer {token.AccessToken}");

                getPortfolioResponse = await Client.GetPortfolioAsync(new GetPortfolioRequest(), headers);

                if (getPortfolioResponse == null || getPortfolioResponse.Error != null)
                {

                    toastService.ShowError($"Unable to fetch portfolio, please try again.");
                    return;
                }
                portfolio.Products = getPortfolioResponse.Portfolio.Products.Select(x => new ProductViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    VolumeActive = x.VolumeActive,
                    VolumeFrozen = x.VolumeFrozen,
                    Spent = x.Spent,
                    Earned = x.Earned,
                    Price = priceDefineService.DefineNormalPrice(x.BestAsk),
                    Estimate = x.Estimate,
                    DeltaAbs = x.DeltaAbs,
                    DeltaRel = x.DeltaRel
                })
                    .ToList();

                portfolio.Total = new Total
                {
                    Spent = getPortfolioResponse.Portfolio.Total.Spent,
                    Earned = getPortfolioResponse.Portfolio.Total.Earned,
                    Estimate = getPortfolioResponse.Portfolio.Total.Estimate,
                    DeltaAbs = getPortfolioResponse.Portfolio.Total.DeltaAbs,
                    DeltaRel = getPortfolioResponse.Portfolio.Total.DeltaRel
                };
            }
            catch (Exception ex)
            {
                toastService.ShowError($"Unable to fetch products, please try again.");
                return;
            }
        }
    }
    public class PortfolioViewModel 
    {
        public List<ProductViewModel> Products { get; set; } = null!;
        public Total Total { get; set; } = null!;
    }
    public class ProductViewModel
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int VolumeActive { get; set; }
        public int VolumeFrozen { get; set; }
        public double Spent { get; set; }
        public double Earned { get; set; }
        public double Price { get; set; }
        public double Estimate { get; set; }
        public double DeltaAbs { get; set; }
        public double DeltaRel { get; set; }
    }

    public class Total
    {
        public double Spent { get; set; }
        public double Earned { get; set; }
        public double Estimate { get; set; }
        public double DeltaAbs { get; set; }
        public double DeltaRel { get; set; }
    }

    

}
