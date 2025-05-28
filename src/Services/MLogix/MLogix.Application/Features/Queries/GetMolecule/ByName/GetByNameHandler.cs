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
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Application.Features.Queries.GetMolecule.ByName
{
    public class GetByNameHandler : IRequestHandler<GetByNameQuery, List<MoleculeVM>>
    {
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetByNameHandler> _logger;
        private readonly IMoleculeAPI _moleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetByNameHandler(
            IMoleculeRepository moleculeRepository,
            IMapper mapper,
            ILogger<GetByNameHandler> logger,
            IMoleculeAPI moleculeAPI,
            IHttpContextAccessor httpContextAccessor)
        {
            _moleculeRepository = moleculeRepository;
            _mapper = mapper;
            _logger = logger;
            _moleculeAPI = moleculeAPI;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<MoleculeVM>> Handle(GetByNameQuery request, CancellationToken cancellationToken)
        {
            var response = new List<MoleculeVM>();
            List<MoleculeBase>? vaultMolecules = null;

            // Retrieve headers from HTTP context
            var headers = _httpContextAccessor.HttpContext?.Request?.Headers?
                .ToDictionary(h => h.Key, h => h.Value.ToString()) ?? new Dictionary<string, string>();

            try
            {
                _logger.LogInformation("Searching for molecules by name: {Name}, Limit: {Limit}", request.Name, request.Limit);

                vaultMolecules = await _moleculeAPI.FindByNameWithFilters(request, headers);

                _logger.LogInformation("Found {Count} molecule(s) matching name: {Name}", vaultMolecules.Count, request.Name);

                foreach (var vaultMolecule in vaultMolecules)
                {
                    try
                    {
                        var dbMolecule = await _moleculeRepository.GetMoleculeByRegistrationId(vaultMolecule.Id);

                        if (dbMolecule == null)
                        {
                            _logger.LogWarning("No local molecule found for registration ID: {Id}", vaultMolecule.Id);
                            continue;
                        }

                        var moleculeVM = _mapper.Map<MoleculeVM>(dbMolecule, opts => opts.Items["WithMeta"] = request.WithMeta);
                        _mapper.Map(vaultMolecule, moleculeVM);

                        // Ensure IDs are consistent
                        moleculeVM.RegistrationId = vaultMolecule.Id;
                        moleculeVM.Id = dbMolecule.Id;

                        var trackables = new List<VMMeta> { moleculeVM };
                        (moleculeVM.PageLastUpdatedDate, moleculeVM.PageLastUpdatedUser) = VMUpdateTracker.CalculatePageLastUpdated(trackables);

                        response.Add(moleculeVM);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing molecule with Vault ID: {Id}", vaultMolecule.Id);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                // If nothing was returned from Vault, fall back to internal DB
                if (vaultMolecules == null || vaultMolecules.Count == 0)
                {
                    _logger.LogWarning("Vault search returned no results for molecule name: {Name}. Falling back to internal DB.", request.Name);

                    var fallbackMolecule = await _moleculeRepository.GetByName(request.Name);
                    if (fallbackMolecule != null)
                    {
                        var moleculeVM = _mapper.Map<MoleculeVM>(fallbackMolecule, opts => opts.Items["WithMeta"] = request.WithMeta);
                        (moleculeVM.PageLastUpdatedDate, moleculeVM.PageLastUpdatedUser) = VMUpdateTracker.CalculatePageLastUpdated(new List<VMMeta> { moleculeVM });
                        response.Add(moleculeVM);
                    }

                    return response;
                }

                _logger.LogError(ex, "Unhandled exception occurred while fetching molecules by name: {Name}", request.Name);
                throw new ResourceNotFoundException(nameof(GetByNameHandler), request.Name);
            }
        }
    }
}
