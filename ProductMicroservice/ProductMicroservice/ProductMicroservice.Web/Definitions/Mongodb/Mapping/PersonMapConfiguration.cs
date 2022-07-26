using AutoMapper;
using ProductMicroservice.Definitions.Mongodb.Models;
using ProductMicroservice.Definitions.Mongodb.ViewModels;

namespace ProductMicroservice.Definitions.Mongodb.Mapping;

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