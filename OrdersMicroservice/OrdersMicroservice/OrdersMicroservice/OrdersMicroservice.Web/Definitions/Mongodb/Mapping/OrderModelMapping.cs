using AutoMapper;
using Orders;
using OrdersMicroservice.Definitions.Mongodb.Models;
using DateTime = System.DateTime;

namespace OrdersMicroservice.Definitions.Mongodb.Mapping;

public class OrderModelMapping : Profile
{
    public OrderModelMapping()
    {
        CreateMap<CreateOrderRequest, OrderModel>()
            .ForMember(d => d.Id, s => s.Ignore())
            .ForMember(d => d.OrderType,
                s => s.MapFrom(x => x.Type == OrderType.Asc ? OrderTypes.Ask : OrderTypes.Bid))
            .ForMember(d => d.Deadline, s => s.MapFrom(x => x.Deadline.ToBsonDateTime()))
            .ForMember(d => d.Status, s => s.Ignore());
    }
}