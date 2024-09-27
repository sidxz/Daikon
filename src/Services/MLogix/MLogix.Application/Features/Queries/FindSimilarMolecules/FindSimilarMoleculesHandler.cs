
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;

namespace MLogix.Application.Features.Queries.FindSimilarMolecules
{
    public class FindSimilarMoleculesHandler(IMoleculeRepository moleculeRepository,
        IMapper mapper, ILogger<FindSimilarMoleculesHandler> logger,
        IMoleculeAPI iMoleculeAPI, IHttpContextAccessor httpContextAccessor) : IRequestHandler<FindSimilarMoleculesQuery, List<SimilarMoleculeVM>>
    {
        private readonly IMoleculeRepository _moleculeRepository = moleculeRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<FindSimilarMoleculesHandler> _logger = logger;
        private readonly IMoleculeAPI _iMoleculeAPI = iMoleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<List<SimilarMoleculeVM>> Handle(FindSimilarMoleculesQuery request, CancellationToken cancellationToken)
        {
            var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());
            try
            {
                _logger.LogInformation("FindSimilarMolecules for SMILES: {0} with threshold: {1} and limit: {2}", request.SMILES, request.Threshold, request.Limit);

                var res = new List<SimilarMoleculeVM>();
                var vaultMolecules = await _iMoleculeAPI.FindSimilar(request, headers);

                _logger.LogInformation("Found {0} similar molecules", vaultMolecules.Count);

                foreach (var vaultMolecule in vaultMolecules)
                {
                    try
                    {
                        var molecule = await _moleculeRepository.GetMoleculeByRegistrationId(vaultMolecule.Id);
                        var similarMoleculeVM = _mapper.Map<SimilarMoleculeVM>(molecule, opts => opts.Items["WithMeta"] = request.WithMeta);
                        _mapper.Map(vaultMolecule, similarMoleculeVM);
                        // Fix Ids
                        similarMoleculeVM.RegistrationId = vaultMolecule.Id;
                        similarMoleculeVM.Id = molecule.Id;

                        res.Add(similarMoleculeVM);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in processing molecule with ID: {0}", vaultMolecule.Id);
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FindSimilarMolecules");
            }

            throw new ResourceNotFoundException(nameof(FindSimilarMoleculesHandler), request.SMILES);

        }
    }
}