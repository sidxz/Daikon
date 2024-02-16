using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Application.Features.Queries.Users.GetUser.VMs;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Queries.Users.GetUser.ById
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, AppUserVM>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByIdHandler> _logger;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAppRoleRepository _appRoleRepository;
        private readonly IAppOrgRepository _appOrgRepository;

        public GetUserByIdHandler(IMapper mapper, ILogger<GetUserByIdHandler> logger, IAppUserRepository appUserRepository, IAppRoleRepository appRoleRepository, IAppOrgRepository appOrgRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appUserRepository = appUserRepository;
            _appRoleRepository = appRoleRepository;
            _appOrgRepository = appOrgRepository;
        }

        async Task<AppUserVM> IRequestHandler<GetUserByIdQuery, AppUserVM>.Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            // fetch the user by id along with its org
            try
            {
                var appUser = await _appUserRepository.GetUserById(request.Id);

                if (appUser == null)
                {
                    throw new ResourceNotFoundException(nameof(AppUser), request.Id);
                }

                var userVM = _mapper.Map<AppUserVM>(appUser);

                if (appUser.AppOrgId != Guid.Empty)
                {
                    var appOrg = await _appOrgRepository.GetOrgById(appUser.AppOrgId);
                    userVM.AppOrg = appOrg;
                }


                return userVM;
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to get user {request.Id}: {e.Message}");
            }

        }
    }
}