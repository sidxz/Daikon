
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.APIResources.UpdateAPIResource
{
    public class UpdateAPIResourceHandler : IRequestHandler<UpdateAPIResourceCommand, APIResource>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateAPIResourceHandler> _logger;
        private readonly IAPIResourceRepository _apiResourceRepository;
        private readonly IAppRoleRepository _appRoleRepository;

        public UpdateAPIResourceHandler(IMapper mapper, ILogger<UpdateAPIResourceHandler> logger, IAPIResourceRepository apiResourceRepository, IAppRoleRepository appRoleRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _apiResourceRepository = apiResourceRepository;
            _appRoleRepository = appRoleRepository;
        }
        public async Task<APIResource> Handle(UpdateAPIResourceCommand request, CancellationToken cancellationToken)
        {
            // check if id is valid
            var existingApi = await _apiResourceRepository.GetAPIResourceById(request.Id);
            if (existingApi == null)
            {
                throw new ResourceNotFoundException(nameof(UpdateAPIResourceHandler), $"API with id {request.Id} was not found.");
            }

            // loop through attached app roles and check if they exist
            foreach (var roleId in request.AttachedAppRoles)
            {
                var appRole = await _appRoleRepository.GetRoleById(roleId);
                if (appRole == null)
                {
                    throw new ResourceNotFoundException(nameof(UpdateAPIResourceHandler), $"AppRole with id {roleId} was not found.");
                }
            }

            // if endpoint is being updated, check if it already exists
            if (existingApi.Endpoint != request.Endpoint)
            {
                var existingApiWithNewEndpoint = await _apiResourceRepository.GetAPIResourceByEndPoint(request.Endpoint);
                if (existingApiWithNewEndpoint != null)
                {
                    throw new DuplicateEntityRequestException(nameof(UpdateAPIResourceHandler), $"API with endpoint {request.Endpoint} already exists.");
                }
            }

            var apiResource = _mapper.Map<APIResource>(request);

            try
            {
                await _apiResourceRepository.UpdateAPIResource(apiResource);
                _logger.LogInformation($"API Resource {apiResource.Name} was updated.");
                return apiResource;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(UpdateAPIResourceHandler)}: {ex.Message}");
                throw;
            }
        }
    }

}