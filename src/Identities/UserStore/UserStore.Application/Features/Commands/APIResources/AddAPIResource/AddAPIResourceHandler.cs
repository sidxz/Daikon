
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.APIResources.AddAPIResource
{
    public class AddAPIResourceHandler : IRequestHandler<AddAPIResourceCommand, APIResource>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AddAPIResourceHandler> _logger;
        private readonly IAPIResourceRepository _apiResourceRepository;
        private readonly IAppRoleRepository _appRoleRepository;

        public AddAPIResourceHandler(IMapper mapper, ILogger<AddAPIResourceHandler> logger, IAPIResourceRepository apiResourceRepository, IAppRoleRepository appRoleRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _apiResourceRepository = apiResourceRepository;
            _appRoleRepository = appRoleRepository;
        }

        public async Task<APIResource> Handle(AddAPIResourceCommand request, CancellationToken cancellationToken)
        {
            // check if api already exists
            var existingApi = await _apiResourceRepository.GetAPIResourceByEndPoint(request.Method, request.Endpoint);
            if (existingApi != null)
            {
                throw new DuplicateEntityRequestException(nameof(AddAPIResourceHandler), $"API with endpoint {request.Method}: {request.Endpoint} already exists.");
            }

            // loop through attached app roles and check if they exist
            foreach (var roleId in request.AttachedAppRoles)
            {
                var appRole = await _appRoleRepository.GetRoleById(roleId);
                if (appRole == null)
                {
                    throw new ResourceNotFoundException(nameof(AddAPIResourceHandler), $"AppRole with id {roleId} was not found.");
                }
            }

            var apiResource = _mapper.Map<APIResource>(request);

            try
            {
                await _apiResourceRepository.AddAPIResource(apiResource);
                _logger.LogInformation($"API Resource {apiResource.Name} was added.");
                return apiResource;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(AddAPIResourceHandler)}: {ex.Message}");
                throw;
            }
        }
    }

}