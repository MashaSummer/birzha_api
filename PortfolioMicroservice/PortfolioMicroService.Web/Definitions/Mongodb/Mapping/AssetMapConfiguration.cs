using AutoMapper;
using PortfolioMicroService.Definitions.Mongodb.Models;
using PortfolioMicroService.Definitions.Mongodb.ViewModels;

namespace PortfolioMicroService.Definitions.Mongodb.Mapping;

public class AssetMapConfiguration : Profile
{
    public AssetMapConfiguration()
    {
        CreateMap<AssetModel, AsssetViewModel>()
            .ForMember(x => x.Id, o => o.MapFrom(c => c.Id))
            .ForMember(x => x.VolumeActive, o => o.MapFrom(c => c.VolumeActive))
            .ForMember(x => x.VolumeFrozen, o => o.MapFrom(c => c.VolumeFrozen));

        CreateMap<ProductAddEvent, UserModel>()
            .ForMember(x => x.Id, o => o.MapFrom(c => c.InvestorId))
            .ForMember(x => x.Asset[0].Id, o => o.MapFrom(c => c.ProductId))
            .ForMember(x => x.Asset[0].StartPrice, o => o.MapFrom(c => c.StartPrice))
            .ForMember(x => x.Asset[0].VolumeActive, o => o.MapFrom(c => c.Volume));
    }
}