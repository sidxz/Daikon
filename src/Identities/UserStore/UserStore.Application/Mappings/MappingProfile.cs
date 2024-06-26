
using AutoMapper;
using UserStore.Application.Features.Commands.APIResources.AddAPIResource;
using UserStore.Application.Features.Commands.APIResources.UpdateAPIResource;
using UserStore.Application.Features.Commands.Orgs.AddOrg;
using UserStore.Application.Features.Commands.Orgs.UpdateOrg;
using UserStore.Application.Features.Commands.Roles.AddRole;
using UserStore.Application.Features.Commands.Roles.UpdateRole;
using UserStore.Application.Features.Commands.Users.AddUser;
using UserStore.Application.Features.Commands.Users.UpdateUser;
using UserStore.Application.Features.Queries.APIResources.ListAPIResources;
using UserStore.Application.Features.Queries.Roles.GetRole.VMs;
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
            CreateMap<AppUser, UpdateUserCommand>().ReverseMap();
            CreateMap<AppUser, AppUserVM>().ReverseMap();

            CreateMap<AppOrg, AddOrgCommand>().ReverseMap();
            CreateMap<AppOrg, UpdateOrgCommand>().ReverseMap();            

            CreateMap<APIResource, AddAPIResourceCommand>().ReverseMap();
            CreateMap<APIResource, UpdateAPIResourceCommand>().ReverseMap();
            CreateMap<APIResource, APIResourceVM>().ReverseMap();

            CreateMap<AppRole, AddRoleCommand>().ReverseMap();
            CreateMap<AppRole, UpdateRoleCommand>().ReverseMap();
            CreateMap<AppRole, AppRoleVM>().ReverseMap();
            CreateMap<AppRole, UpdateRoleDTO>().ReverseMap();
        }
        
    }
}