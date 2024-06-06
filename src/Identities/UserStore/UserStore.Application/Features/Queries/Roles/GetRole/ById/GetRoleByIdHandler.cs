using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Application.Features.Queries.Roles.GetRole.VMs;
using UserStore.Application.Features.Queries.Users.GetUser.VMs;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Queries.Roles.GetRole.ById
{
    public class GetRoleByIdHandler : IRequestHandler<GetRoleByIdQuery, AppRoleVM>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<GetRoleByIdHandler> _logger;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAppRoleRepository _appRoleRepository;
        private readonly IAppOrgRepository _appOrgRepository;

        public GetRoleByIdHandler(IMapper mapper, ILogger<GetRoleByIdHandler> logger, IAppUserRepository appUserRepository, IAppRoleRepository appRoleRepository, IAppOrgRepository appOrgRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appUserRepository = appUserRepository;
            _appRoleRepository = appRoleRepository;
            _appOrgRepository = appOrgRepository;
        }

        async Task<AppRoleVM> IRequestHandler<GetRoleByIdQuery, AppRoleVM>.Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            // fetch the role by id along with its users
            try
            {
                var appRole = await _appRoleRepository.GetRoleById(request.Id) ?? throw new ResourceNotFoundException(nameof(AppUser), request.Id);
                
                var roleVM = _mapper.Map<AppRoleVM>(appRole);

                // fetch the users of the role
                var users = await _appUserRepository.GetUsersByRole(appRole.Id);

                roleVM.Users = users.Select(u => u.Id).ToList();


                return roleVM;
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to get role {request.Id}: {e.Message}");
            }

        }
    }
}