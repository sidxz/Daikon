
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Queries.Roles.ListRoles
{
    public class ListRolesHandler : IRequestHandler<ListRolesQuery, List<AppRole>>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ListRolesHandler> _logger;
        private readonly IAppRoleRepository _appRoleRepository;

        public ListRolesHandler(IMapper mapper, ILogger<ListRolesHandler> logger, IAppRoleRepository appRoleRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appRoleRepository = appRoleRepository;
        }
        public async Task<List<AppRole>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
        {
            try 
            {
                var roles = await _appRoleRepository.GetRolesList();
                return roles;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while fetching roles.");
                throw;
            }
        }
    }
}