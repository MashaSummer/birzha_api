using AutoMapper;
using BalanceMicroservice.Domain;
using BalanceMicroservice.Web.Definitions.Mapping;
using BalanceMicroservice.Web.Endpoints.EventItemsEndpoints.ViewModels;
using Calabonga.UnitOfWork;

namespace BalanceMicroservice.Web.Endpoints.EventItemsEndpoints
{
    /// <summary>
    /// Mapper Configuration for entity EventItem
    /// </summary>
    public class EventItemMapperConfiguration : Profile
    {
        /// <inheritdoc />
        public EventItemMapperConfiguration()
        {
            CreateMap<EventItemCreateViewModel, EventItem>()
                .ForMember(x => x.Id, o => o.Ignore());

            CreateMap<EventItem, EventItemViewModel>();

            CreateMap<EventItem, EventItemUpdateViewModel>();

            CreateMap<EventItemUpdateViewModel, EventItem>()
                .ForMember(x => x.CreatedAt, o => o.Ignore())
                .ForMember(x => x.ThreadId, o => o.Ignore())
                .ForMember(x => x.ExceptionMessage, o => o.Ignore());

            CreateMap<IPagedList<EventItem>, IPagedList<EventItemViewModel>>()
                .ConvertUsing<PagedListConverter<EventItem, EventItemViewModel>>();
        }
    }
}