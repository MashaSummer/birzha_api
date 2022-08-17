using AutoMapper;
using ProductGrpc;
using ProductMicroservice.Mongodb.Models;

namespace ProductMicroservice.Mongodb.Mapping;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<ProductModel, ProductGrpc.ProductArray.Types.Product>();
        CreateMap<ChangePortfolioRequest, ProductModel>()
            .ForMember(d => d.Id, s => s.Ignore())
            .ForMember(d => d.InvestorId, s => s.MapFrom(x => x.InvestorId));
    }
}