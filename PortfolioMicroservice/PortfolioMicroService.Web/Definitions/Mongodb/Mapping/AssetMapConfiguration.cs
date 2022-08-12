using AutoMapper;
using PortfolioMicroService.Definitions.Mongodb.Models;

namespace PortfolioMicroService.Definitions.Mongodb.Mapping;

public class AssetMapConfiguration : Profile
{
    public AssetMapConfiguration()
    {
       
        CreateMap<AssetModel, AssetArray.Types.Asset>()
           .ForMember(x => x.Id, o => o.MapFrom(c => c.Id))
           .ForMember(x => x.VolumeActive, o => o.MapFrom(c => c.VolumeActive))
           .ForMember(x => x.VolumeFrozen, o => o.MapFrom(c => c.VolumeFrozen))
           .ForMember(x => x.StartPrice, o => o.MapFrom(o => o.StartPrice));
        CreateMap<ProductAddEvent, AssetModel>()
            .ForMember(x => x.Id, o => o.MapFrom(c => c.ProductId))
            .ForMember(x => x.StartPrice, o => o.MapFrom(c => c.StartPrice))
            .ForMember(x => x.VolumeActive, o => o.MapFrom(c => c.Volume))
            .ForMember(x => x.VolumeFrozen, o => o.Ignore());
    }
}