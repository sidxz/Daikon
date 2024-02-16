

using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Orgs.AddOrg
{
    public class AddOrgHandler : IRequestHandler<AddOrgCommand, AppOrg>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AddOrgHandler> _logger;
        private readonly IAppOrgRepository _appOrgRepository;

        public AddOrgHandler(IMapper mapper, ILogger<AddOrgHandler> logger, IAppOrgRepository appOrgRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appOrgRepository = appOrgRepository;
        }

        public async Task<AppOrg> Handle(AddOrgCommand request, CancellationToken cancellationToken)
        {
            // check if org already exists
            var existingOrg = await _appOrgRepository.GetOrgByName(request.Name);
            if (existingOrg != null)
            {
                throw new DuplicateEntityRequestException(nameof(AddOrgHandler), $"Org with name {request.Name} already exists.");
            }

            // check if org alias already exists
            existingOrg = await _appOrgRepository.GetOrgByAlias(request.Alias);
            if (existingOrg != null)
            {
                throw new DuplicateEntityRequestException(nameof(AddOrgHandler), $"Org with alias {request.Alias} already exists.");
            }

            var newOrg = _mapper.Map<AppOrg>(request);
            newOrg.CreatedAt = DateTime.UtcNow;

            try
            {
                await _appOrgRepository.AddOrg(newOrg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding org.");
                throw new InvalidOperationException("An error occurred while adding org.", ex);
            }

            return newOrg;
        }
    }
}