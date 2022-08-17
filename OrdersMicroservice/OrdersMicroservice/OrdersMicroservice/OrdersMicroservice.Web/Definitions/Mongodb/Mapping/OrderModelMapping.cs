using AutoMapper;
using Orders;
using OrdersMicroservice.Definitions.Mongodb.Models;
using DateTime = System.DateTime;

namespace OrdersMicroservice.Definitions.Mongodb.Mapping;

public class OrderModelMapping : Profile
{
    public OrderModelMapping()
    {
        CreateMap<Order, OrderModel>()
            .ForMember(d => d.Id, s => s.Ignore())
            .ForMember(d => d.OrderType,
                s => s.MapFrom(x => x.Type == OrderType.Ask ? OrderTypes.Ask : OrderTypes.Bid))
            .ForMember(d => d.Deadline, s => s.MapFrom(x => x.Deadline.ToDateTime()))
            .ForMember(d => d.Status, s => s.Ignore())
            .ForMember(d => d.SubmissionTime, s => s.Ignore());
    }
}