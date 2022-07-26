using AutoMapper;
using ProductMicroservice.Definitions.Mongodb.Models;
using ProductMicroservice.Definitions.Mongodb.ViewModels;

namespace ProductMicroservice.Definitions.Mongodb.Mapping;

public class ProductMapConfiguration : Profile
{
    public ProductMapConfiguration()
    {
        CreateMap<ProductModel, ProductViewModel>()
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