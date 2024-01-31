

using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Orgs.UpdateOrg
{
    public class UpdateOrgHandler : IRequestHandler<UpdateOrgCommand, AppOrg>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateOrgHandler> _logger;
        private readonly IAppOrgRepository _appOrgRepository;

        public UpdateOrgHandler(IMapper mapper, ILogger<UpdateOrgHandler> logger, IAppOrgRepository appOrgRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appOrgRepository = appOrgRepository;
        }

        public async Task<AppOrg> Handle(UpdateOrgCommand request, CancellationToken cancellationToken)
        {

            // Fetch org
            var org = await _appOrgRepository.GetOrgById(request.Id);

            if (org == null)
            {
                throw new ResourceNotFoundException(nameof(AppOrg), request.Id);
            }

            // if name is changed, check if it is already in use
            if (request.Name != org.Name)
            {
                var existingOrg = await _appOrgRepository.GetOrgByName(request.Name);
                if (existingOrg != null)
                {
                    throw new InvalidOperationException($"Name {request.Name} is already in use.");
                }
            }

            // if alias is changed, check if it is already in use
            if (request.Alias != org.Alias)
            {
                var existingOrg = await _appOrgRepository.GetOrgByAlias(request.Alias);
                if (existingOrg != null)
                {
                    throw new InvalidOperationException($"Alias {request.Alias} is already in use.");
                }
            }

            // Update org
            _mapper.Map(request, org);

            try
            {
                await _appOrgRepository.UpdateOrg(org);
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to update org {org.Id}: {e.Message}");
            }

            return org;
        }
    }
}