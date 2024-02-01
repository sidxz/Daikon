
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;

namespace UserStore.Application.Features.Commands.APIResources.DeleteAPIResource
{
    public class DeleteAPIResourceHandler : IRequestHandler<DeleteAPIResourceCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteAPIResourceHandler> _logger;
        private readonly IAPIResourceRepository _apiResourceRepository;
        private readonly IAppRoleRepository _appRoleRepository;

        public DeleteAPIResourceHandler(IMapper mapper, ILogger<DeleteAPIResourceHandler> logger, IAPIResourceRepository apiResourceRepository, IAppRoleRepository appRoleRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _apiResourceRepository = apiResourceRepository;
            _appRoleRepository = appRoleRepository;
        }
        public async Task<Unit> Handle(DeleteAPIResourceCommand request, CancellationToken cancellationToken)
        {
            // check if id is valid
            var existingApi = await _apiResourceRepository.GetAPIResourceById(request.Id);
            if (existingApi == null)
            {
                throw new ResourceNotFoundException(nameof(DeleteAPIResourceHandler), $"API with id {request.Id} was not found.");
            }

            try
            {
                await _apiResourceRepository.DeleteAPIResource(existingApi.Id);
                _logger.LogInformation($"API Resource {existingApi.Name} was deleted.");
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

        }
    }

}