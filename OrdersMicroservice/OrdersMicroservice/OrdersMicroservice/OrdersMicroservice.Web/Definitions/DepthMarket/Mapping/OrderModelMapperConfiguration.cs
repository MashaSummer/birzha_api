using AutoMapper;
using OrdersMicroservice.Definitions.Mongodb.Models;

namespace OrdersMicroservice.Definitions.DepthMarket.Mapping
{
    public class OrderModelConfiguration : Profile
    {
        
        public OrderModelConfiguration()
        {
            CreateMap<OrderModel, MarketModel>()
                .ForMember(
                    dest => dest.ProductId,
                    from => from.MapFrom(x => x.ProductId)
                )
                .ForMember(
                    dest => dest.OrderId,
                    from => from.MapFrom(x => x.Id)
                )
                .ForMember(
                    dest => dest.Volume,
                    from => from.MapFrom(x => x.Volume)
                )
                .ForMember(
                    dest => dest.Price,
                    from => from.MapFrom(x => x.Price)
                )
                .ForMember(
                    dest => dest.OnlyFullExecution,
                    from => from.MapFrom(x => x.OnlyFullExecution)
                )
                .ForMember(
                    dest => dest.SubmissionTime,
                    from => from.MapFrom(x => x.SubmittionTime)
                );

        }
    }
}
