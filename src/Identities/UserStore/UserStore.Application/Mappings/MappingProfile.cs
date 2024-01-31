
using AutoMapper;
using UserStore.Application.Features.Commands.Orgs.AddOrg;
using UserStore.Application.Features.Commands.Users.AddUser;
using UserStore.Application.Features.Queries.Users.GetUser.VMs;
using UserStore.Domain.Entities;

namespace UserStore.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppOrg, AddOrgCommand>().ReverseMap();
            CreateMap<AppUser, AddUserCommand>().ReverseMap();
            CreateMap<AppUser, AppUserVM>().ReverseMap();
        }
        
    }
}