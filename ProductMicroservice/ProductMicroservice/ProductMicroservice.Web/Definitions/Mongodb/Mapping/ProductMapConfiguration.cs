using AutoMapper;
using ProductGrpc;
using ProductMicroservice.Definitions.Mongodb.Models;

namespace ProductMicroservice.Definitions.Mongodb.Mapping;

public class ProductMapConfiguration : Profile
{
    public ProductMapConfiguration()
    {
        CreateMap<ProductModel, ProductArray.Types.Product>()
            .ForMember(d => d.Id, 
                s => s.MapFrom(x => x.Id))
            .ForMember(d => d.Name, 
                s => s.MapFrom(x => x.Name))
            .ForMember(d => d.BestAsk, 
                s => s.MapFrom(x => x.BestAsk))
            .ForMember(d => d.BestBid, 
                s => s.MapFrom(x => x.BestBid));
    }
}