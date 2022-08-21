﻿using BlazorClient.Attributes;
using BlazorClient.Infrastructure;
using Blazored.Toast.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Orders;
using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Components
{
    public class PostOrderModel : ComponentBase
    {
        public Guid Guid = Guid.NewGuid();
        public string ProductId { get; set; } = null!;
        public OrderType OrderType { get; set; }
        public string ModalDisplay = "none;";
        public string ModalClass = "fade";
        public bool ShowBackdrop = false;
        public PostOrderViewModel postOrderViewModel = new();
        [Inject] PriceDefineService priceDefineService { get; set; }
        [Inject] IConfiguration config { get; set; }
        [Inject] IToastService toastService { get; set; }

        public async Task HandleValidSubmitAsync()
        {
            var t = postOrderViewModel;
            var address = config["FacadeBaseURL"];
            CreateOrderResponse createOrderResponse = new();
            try
            {
                var channel = GrpcChannel.ForAddress(address);

                var client = new OrdersService.OrdersServiceClient(channel);
                var orderDetail = new Order()
                {
                    ProductId = ProductId,
                    Volume = postOrderViewModel.Volume,
                    Price = priceDefineService.DefinePrice(postOrderViewModel.Price),
                    OnlyFullExecution = postOrderViewModel.OnlyFullExecution,
                    SubmissionTime = Timestamp.FromDateTime(postOrderViewModel.SubmissionTime),
                    Deadline = Timestamp.FromDateTime(postOrderViewModel.Deadline),
                    InvestorId = null
                };
                var postOrderRequest = new CreateOrderRequest() { OrderDetail = orderDetail };
                
                createOrderResponse = await client.CreateOrderAsync(postOrderRequest);

                if (createOrderResponse == null || createOrderResponse.Error != null)
                {
                    Close();
                    toastService.ShowError($"Enable to post order");
                    return;
                }
            }
            catch (Exception ex)
            {
                Close();
                toastService.ShowError($"Enable to post order");
                return;
            }
            Close();
            toastService.ShowSuccess($"Order succesfully added");  
        }


        public void Open(string productId, OrderType orderType)
        {
            ProductId = productId;
            OrderType = orderType;
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
    public class PostOrderViewModel
    {
        public string ProductId { get; set; } = null!;
        [Required]
        public int Volume { get; set; }
        [Required]
        [BasePrice(ErrorMessage = "Invalid format, example 123.45")]
        public double Price { get; set; }
        [Required]
        public bool OnlyFullExecution { get; set; }
        public DateTime SubmissionTime { get; set; } = DateTime.UtcNow;
        [Required]
        [DeadlineValidation]
        public DateTime Deadline { get; set; }
    }
}