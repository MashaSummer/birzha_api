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
    }
}