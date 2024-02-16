
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Application.Features.Queries.Users.GetUser.VMs;

namespace UserStore.Application.Features.Queries.Users.ListUsers
{
    public class ListUsersHandler : IRequestHandler<ListUsersQuery, List<AppUserVM>>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ListUsersHandler> _logger;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAppRoleRepository _appRoleRepository;
        private readonly IAppOrgRepository _appOrgRepository;

        public ListUsersHandler(IMapper mapper, ILogger<ListUsersHandler> logger, IAppUserRepository appUserRepository, IAppRoleRepository appRoleRepository, IAppOrgRepository appOrgRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appUserRepository = appUserRepository;
            _appRoleRepository = appRoleRepository;
            _appOrgRepository = appOrgRepository;
        }
        public async Task<List<AppUserVM>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var appUserList = new List<AppUserVM>();

                var users = await _appUserRepository.GetUsersList();

                foreach (var user in users)
                {
                    var userVM = _mapper.Map<AppUserVM>(user);

                    if (user.AppOrgId != Guid.Empty)
                    {
                        var appOrg = await _appOrgRepository.GetOrgById(user.AppOrgId);
                        userVM.AppOrg = appOrg;
                    }

                    appUserList.Add(userVM);
                }
                
                return appUserList;
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to get users {e.Message}");
            }
        }
    }
}