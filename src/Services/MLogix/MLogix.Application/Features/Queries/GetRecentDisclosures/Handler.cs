using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Daikon.Shared.VM.MLogix;
using MediatR;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using CQRS.Core.Extensions;
using CQRS.Core.Domain;

namespace MLogix.Application.Features.Queries.GetRecentDisclosures
{
    public class Handler : IRequestHandler<GetRecentDisclosuresQuery, List<MoleculeVM>>
    {
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IMapper _mapper;
        private readonly IMoleculeAPI _iMoleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IMoleculeRepository moleculeRepository,
            IMapper mapper,
            IMoleculeAPI iMoleculeAPI,
            IHttpContextAccessor httpContextAccessor,
            ILogger<Handler> logger)
        {
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _iMoleculeAPI = iMoleculeAPI ?? throw new ArgumentNullException(nameof(iMoleculeAPI));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<MoleculeVM>> Handle(GetRecentDisclosuresQuery request, CancellationToken cancellationToken)
        {
            var molecules = await _moleculeRepository.GetDisclosedMolecules(request.StartDate, request.EndDate);

            if (molecules == null || !molecules.Any())
            {
                return [];
            }

            var moleculeVMs = _mapper.Map<List<MoleculeVM>>(molecules, opts => opts.Items["WithMeta"] = request.WithMeta);

            try
            {
                var headers = _httpContextAccessor.HttpContext.Request.Headers
                    .ToDictionary(h => h.Key, h => h.Value.ToString());

                var registrationIds = moleculeVMs
                    .Where(m => m.RegistrationId != Guid.Empty)
                    .Select(m => m.RegistrationId)
                    .ToList();

                var vaultMolecules = await _iMoleculeAPI.GetMoleculesByIds(registrationIds, headers);
                _logger.LogInformation("Fetched {Count} molecules from vault API", vaultMolecules.Count);

                foreach (var vaultMolecule in vaultMolecules)
                {
                    var moleculeVm = moleculeVMs.FirstOrDefault(vm => vm.RegistrationId == vaultMolecule.Id);
                    if (moleculeVm != null)
                    {
                        var id = moleculeVm.Id; // Preserve DB Id
                        _mapper.Map(vaultMolecule, moleculeVm); // Overwrite with vault values
                        moleculeVm.Id = id;
                        moleculeVm.RegistrationId = vaultMolecule.Id;

                        var trackableEntities = new List<VMMeta> { moleculeVm };
                        (moleculeVm.PageLastUpdatedDate, moleculeVm.PageLastUpdatedUser)
                            = VMUpdateTracker.CalculatePageLastUpdated(trackableEntities);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enriching recent disclosures with vault data");
            }

            return moleculeVMs;
        }
    }
}
