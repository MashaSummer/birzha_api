using AutoMapper;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Definitions.Mongodb.ViewModels;

namespace OrdersMicroservice.Definitions.Mongodb.Mapping;

public class PersonMapConfiguration : Profile
{
    public PersonMapConfiguration()
    {
        CreateMap<PersonModel, PersonViewModel>()
            .ForMember(x => x.FirstName, o =>
                o.MapFrom(c => c.FirstName))
            .ForMember(x => x.LastName, o =>
                o.MapFrom(c => c.LastName));
    }
}