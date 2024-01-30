
using AutoMapper;
using UserStore.Application.Features.Commands.Orgs.AddOrg;
using UserStore.Application.Features.Commands.Users.AddUser;

namespace UserStore.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domain.Entities.AppOrg, AddOrgCommand>().ReverseMap();
            CreateMap<Domain.Entities.AppUser, AddUserCommand>().ReverseMap();
        }
        
    }
}