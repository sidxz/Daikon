using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Exceptions;
using CQRS.Core.Extensions;
using Daikon.Shared.VM.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;

namespace MLogix.Application.Features.Queries.GetMolecules.ByIDs
{
    public class GetMoleculeByIDsHandler(IMoleculeRepository moleculeRepository, IMapper mapper,
                                   ILogger<GetMoleculeByIDsHandler> logger, IMoleculeAPI iMoleculeAPI,
                                   IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetMoleculeByIDsQuery, List<MoleculeVM>>
    {
        private readonly IMoleculeRepository _moleculeRepository = moleculeRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetMoleculeByIDsHandler> _logger = logger;
        private readonly IMoleculeAPI _iMoleculeAPI = iMoleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<List<MoleculeVM>> Handle(GetMoleculeByIDsQuery request, CancellationToken cancellationToken)
        {
            var headers = _httpContextAccessor.HttpContext.Request.Headers
                .ToDictionary(h => h.Key, h => h.Value.ToString());

            try
            {
                // Fetch molecules from the repository
                var molecules = await _moleculeRepository.GetMoleculesByIds(request.IDs);
                if (molecules == null || molecules.Count == 0)
                {
                    return [];
                }

                var moleculeVMs = _mapper.Map<List<MoleculeVM>>(molecules);

                // Extract registration IDs for the API call
                var registrationIds = molecules.Select(m => m.RegistrationId).ToList();

                // Make a single API call to get all the molecules by registration IDs
                var vaultMolecules = await _iMoleculeAPI.GetMoleculesByIds(registrationIds, headers);
                _logger.LogInformation("Fetched {Count} molecules from vault API", vaultMolecules.Count);

                // Map the fetched vault molecules to the corresponding VM
                foreach (var vaultMolecule in vaultMolecules)
                {
                    var moleculeVm = moleculeVMs.FirstOrDefault(vm => vm.RegistrationId == vaultMolecule.Id);

                    if (moleculeVm != null)
                    {
                        var moleculeId = moleculeVm.Id;
                        _mapper.Map(vaultMolecule, moleculeVm);
                        // fix ids
                        moleculeVm.Id = moleculeId;
                        moleculeVm.RegistrationId = vaultMolecule.Id;

                        var trackableEntities = new List<VMMeta> { moleculeVm };
                        (moleculeVm.PageLastUpdatedDate, moleculeVm.PageLastUpdatedUser)
                                    = VMUpdateTracker.CalculatePageLastUpdated(trackableEntities);
                    }
                }

                return moleculeVMs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching molecules by IDs");
                throw new ApplicationException("Error fetching molecules", ex);
            }
        }
    }
}
