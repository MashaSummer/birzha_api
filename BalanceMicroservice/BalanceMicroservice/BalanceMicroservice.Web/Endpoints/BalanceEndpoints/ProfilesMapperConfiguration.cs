using AutoMapper;
using BalanceMicroservice.Web.Endpoints.ProfileEndpoints.ViewModels;
using Calabonga.Microservices.Core;
using System.Security.Claims;

namespace BalanceMicroservice.Web.Endpoints.ProfileEndpoints
{
    /// <summary>
    /// Mapper Configuration for entity ApplicationUser
    /// </summary>
    public class ProfilesMapperConfiguration : Profile
    {
        /// <inheritdoc />
        public ProfilesMapperConfiguration()
            => CreateMap<ClaimsIdentity, BalanceViewModel>()
                .ForMember(x => x.Id, o => o.MapFrom(claims => ClaimsHelper.GetValue<Guid>(claims, ClaimTypes.Name)))
                .ForMember(x => x.Balance, o => o.MapFrom(claims => ClaimsHelper.GetValue<string>(claims, ClaimTypes.UserData)));
    }
}