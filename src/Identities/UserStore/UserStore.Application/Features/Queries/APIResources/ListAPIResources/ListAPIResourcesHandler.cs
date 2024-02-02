
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;

namespace UserStore.Application.Features.Queries.APIResources.ListAPIResources
{
    public class ListAPIResourcesHandler : IRequestHandler<ListAPIResourcesQuery, List<APIResourceVM>>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ListAPIResourcesHandler> _logger;
        private readonly IAPIResourceRepository _apiResourceRepository;
        private readonly IAppRoleRepository _appRoleRepository;

        public ListAPIResourcesHandler(IMapper mapper, ILogger<ListAPIResourcesHandler> logger, IAPIResourceRepository apiResourceRepository, IAppRoleRepository appRoleRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _apiResourceRepository = apiResourceRepository;
            _appRoleRepository = appRoleRepository;
        }
        public async Task<List<APIResourceVM>> Handle(ListAPIResourcesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var apiResourceList = new List<APIResourceVM>();

                var apiResources = await _apiResourceRepository.GetAPIResourcesList();

                // for each api resource, get the attached app roles
                foreach (var apiResource in apiResources)
                {
                    var apiResourceVM = _mapper.Map<APIResourceVM>(apiResource);
                    apiResourceVM.AttachedAppRolesExpanded = await _appRoleRepository.GetRolesByIds(apiResource.AttachedAppRoles);
                    apiResourceList.Add(apiResourceVM);
                }

                return apiResourceList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}