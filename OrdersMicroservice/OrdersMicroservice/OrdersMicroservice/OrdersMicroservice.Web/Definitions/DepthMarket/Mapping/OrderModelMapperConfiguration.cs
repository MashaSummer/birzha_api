using AutoMapper;
using OrdersMicroservice.Definitions.Mongodb.Models;

namespace OrdersMicroservice.Definitions.DepthMarket.Mapping
{
    public class OrderModelConfiguration : Profile
    {
        
        public OrderModelConfiguration()
        {
            CreateMap<OrderModel, MarketModel>()
                .ForMember(dest => dest.Id, from => from.Ignore())
                .ForMember(
                    dest => dest.OrderId,
                    from => from.MapFrom(x => x.Id)
                );

        }
    }
}
