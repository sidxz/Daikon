using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Queries.Orgs.GetOrg.ById
{
    public class GetOrgByIdHandler : IRequestHandler<GetOrgByIdQuery, AppOrg>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<GetOrgByIdHandler> _logger;
        private readonly IAppOrgRepository _appOrgRepository;

        public GetOrgByIdHandler(IMapper mapper, ILogger<GetOrgByIdHandler> logger, IAppOrgRepository appOrgRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appOrgRepository = appOrgRepository;
        }
        public Task<AppOrg> Handle(GetOrgByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var org = _appOrgRepository.GetOrgById(request.Id);

                if (org == null)
                {
                    throw new ResourceNotFoundException(nameof(AppOrg), request.Id);
                }

                return org;
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to get org {request.Id}: {e.Message}");
            }
        }
    }
}