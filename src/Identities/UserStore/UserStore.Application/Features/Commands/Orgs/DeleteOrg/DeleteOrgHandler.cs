
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Orgs.DeleteOrg
{
    public class DeleteOrgHandler : IRequestHandler<DeleteOrgCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteOrgHandler> _logger;
        private readonly IAppOrgRepository _appOrgRepository;

        public DeleteOrgHandler(IMapper mapper, ILogger<DeleteOrgHandler> logger, IAppOrgRepository appOrgRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appOrgRepository = appOrgRepository;
        }



        public async Task<Unit> Handle(DeleteOrgCommand request, CancellationToken cancellationToken)
        {
            // Fetch user
            var org = await _appOrgRepository.GetOrgById(request.Id);

            if (org == null)
            {
                throw new ResourceNotFoundException(nameof(AppOrg), request.Id);
            }

            // Delete user
            try
            {
                await _appOrgRepository.DeleteOrg(request.Id);
                return Unit.Value;
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to delete org {org.Id}: {e.Message}");
            }

        }
    }
}