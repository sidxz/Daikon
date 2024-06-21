
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.Features.Queries.GetMolecule;

namespace MLogix.Application.Features.Queries.FindSimilarMolecules
{
    public class FindSimilarMoleculesHandler : IRequestHandler<FindSimilarMoleculesQuery, List<MoleculeVM>>
    {
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FindSimilarMoleculesHandler> _logger;

        private readonly IMolDbAPIService _molDbAPIService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public FindSimilarMoleculesHandler(IMoleculeRepository moleculeRepository,
            IMapper mapper, ILogger<FindSimilarMoleculesHandler> logger,
            IMolDbAPIService molDbAPIService, IHttpContextAccessor httpContextAccessor)
        {
            _moleculeRepository = moleculeRepository;
            _mapper = mapper;
            _logger = logger;
            _molDbAPIService = molDbAPIService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<MoleculeVM>> Handle(FindSimilarMoleculesQuery request, CancellationToken cancellationToken)
        {
            var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());
            try
            {
                _logger.LogInformation("FindSimilarMolecules for SMILES: {0} with threshold: {1} and max results: {2}", request.SMILES, request.SimilarityThreshold, request.MaxResults);

                var res = new List<MoleculeVM>();
                var molDbMolecules = await _molDbAPIService.FindSimilar(request.SMILES, (float)request.SimilarityThreshold, request.MaxResults, headers);

                _logger.LogInformation("Found {0} similar molecules", molDbMolecules.Count);

                foreach (var molDbMolecule in molDbMolecules)
                {
                    try
                    {
                        var molecule = await _moleculeRepository.GetMoleculeByRegistrationId(molDbMolecule.Id);
                        var moleculeVm = _mapper.Map<MoleculeVM>(molecule, opts => opts.Items["WithMeta"] = request.WithMeta);

                        //moleculeVm.Smiles = molDbMolecule.Smiles;
                        moleculeVm.Smiles = molecule.RequestedSMILES ?? molDbMolecule.Smiles ?? "";
                        moleculeVm.SmilesCanonical = molDbMolecule.SmilesCanonical;
                        moleculeVm.MolecularWeight = (float)Math.Round(molDbMolecule.MolecularWeight, 2);
                        moleculeVm.TPSA = (float)Math.Round(molDbMolecule.TPSA, 2);
                        moleculeVm.Similarity = (float)Math.Round(molDbMolecule.Similarity, 2);
                        res.Add(moleculeVm);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in processing molecule with ID: {0}", molDbMolecule.Id);
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FindBySMILES");
            }

            throw new ResourceNotFoundException(nameof(FindSimilarMoleculesHandler), request.SMILES);

        }
    }
}