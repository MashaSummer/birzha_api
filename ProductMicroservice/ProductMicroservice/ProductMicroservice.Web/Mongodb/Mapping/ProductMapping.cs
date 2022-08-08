using AutoMapper;
using ProductMicroservice.Mongodb.Models;

namespace ProductMicroservice.Mongodb.Mapping;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<ProductModel, ProductGrpc.ProductArray.Types.Product>();
    }
}