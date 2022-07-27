using AutoMapper;
using ProductGrpc;
using ProductMicroservice.Definitions.Mongodb.Models;

namespace ProductMicroservice.Definitions.Mongodb.Mapping;

public class ProductMapConfiguration : Profile
{
    public ProductMapConfiguration()
    {
        CreateMap<ProductModel, ProductArray.Types.Product>();
    }
}