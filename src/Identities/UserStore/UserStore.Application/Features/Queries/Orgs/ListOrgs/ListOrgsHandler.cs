
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Queries.Orgs.ListOrgs
{
    public class ListOrgsHandler : IRequestHandler<ListOrgsQuery, List<AppOrg>>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ListOrgsHandler> _logger;
        private readonly IAppOrgRepository _appOrgRepository;

        public ListOrgsHandler(IMapper mapper, ILogger<ListOrgsHandler> logger, IAppOrgRepository appOrgRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appOrgRepository = appOrgRepository;
        }

        public async Task<List<AppOrg>> Handle(ListOrgsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var orgs = await _appOrgRepository.GetOrgsList();

                return orgs;
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to get orgs {e.Message}");
            }
        }
    }
}