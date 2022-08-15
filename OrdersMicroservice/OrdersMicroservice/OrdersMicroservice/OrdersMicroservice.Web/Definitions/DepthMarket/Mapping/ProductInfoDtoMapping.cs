using AutoMapper;
using Orders;
using OrdersMicroservice.Definitions.DepthMarket.Dto;

namespace OrdersMicroservice.Definitions.DepthMarket.Mapping;

public class ProductInfoDtoMapping : Profile
{
    public ProductInfoDtoMapping()
    {
        CreateMap<UserProductInfoDto, UserProductInfo>();
    }
}