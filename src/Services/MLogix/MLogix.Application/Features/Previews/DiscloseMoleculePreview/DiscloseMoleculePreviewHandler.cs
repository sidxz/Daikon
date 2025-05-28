
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;

namespace MLogix.Application.Features.Previews.DiscloseMoleculePreview
{
    public class DiscloseMoleculePreviewHandler : IRequestHandler<DiscloseMoleculePreviewQuery, List<DiscloseMoleculePreviewDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DiscloseMoleculePreviewHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IMoleculeAPI _moleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DiscloseMoleculePreviewHandler(
            IMapper mapper,
            ILogger<DiscloseMoleculePreviewHandler> logger,
            IMoleculeRepository moleculeRepository,
            IMoleculeAPI moleculeAPI,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _moleculeAPI = moleculeAPI ?? throw new ArgumentNullException(nameof(moleculeAPI));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<List<DiscloseMoleculePreviewDTO>> Handle(DiscloseMoleculePreviewQuery request, CancellationToken cancellationToken)
        {
            if (request.Queries == null || request.Queries.Count == 0)
            {
                throw new ArgumentException("At least one query must be provided.");
            }

            var results = new List<DiscloseMoleculePreviewDTO>();
            var headers = GetRequestHeaders();

            // Process queries in parallel for better performance
            await Parallel.ForEachAsync(request.Queries, cancellationToken, async (query, ct) =>
            {
                var previewDto = await ProcessQueryAsync(query, headers, ct);
                if (previewDto != null)
                {
                    lock (results) // Ensure thread-safe access to the list
                    {
                        results.Add(previewDto);
                    }
                }
            });

            return results;
        }

        private Dictionary<string, string> GetRequestHeaders()
        {
            return _httpContextAccessor.HttpContext?.Request?.Headers
                .ToDictionary(h => h.Key, h => h.Value.ToString()) ?? new Dictionary<string, string>();
        }

        private async Task<DiscloseMoleculePreviewDTO> ProcessQueryAsync(QueryItem query, Dictionary<string, string> headers, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query.Name) || string.IsNullOrWhiteSpace(query.SMILES))
            {
                _logger.LogWarning("Skipping query due to missing required fields. Name: {Name}, SMILES: {SMILES}", query.Name, query.SMILES);
                return null;
            }

            var previewDto = new DiscloseMoleculePreviewDTO
            {
                Name = query.Name,
                RequestedSMILES = query.SMILES,
                Errors = [],
                IsValid = true
            };

            // Check if the molecule exists in MLOGIX
            var existingMolecule = await _moleculeRepository.GetByName(query.Name);
            if (existingMolecule == null)
            {
                _logger.LogWarning("Molecule not found in MLOGIX: {Name}", query.Name);
                previewDto.Errors.Add($"The Molecule Name '{query.Name}' is not found in MLOGIX.");
                previewDto.IsValid = false;
                return previewDto;
            }

            if (existingMolecule.RequestedSMILES != null)
            {
                _logger.LogWarning("Molecule already disclosed: {Name} with SMILES {Smiles}", query.Name, existingMolecule.RequestedSMILES);
                previewDto.Errors.Add($"The Molecule Name '{query.Name}' has already been disclosed with the SMILES '{existingMolecule.RequestedSMILES}'.");
                previewDto.IsValid = false;
                return previewDto;
            }
            previewDto.Id = existingMolecule.Id;

            // Check if the SMILES exists in DaikonChemVault
            var vaultMolecule = await _moleculeAPI.GetMoleculeBySMILES(query.SMILES, headers);
            if (vaultMolecule != null)
            {
                _logger.LogWarning("SMILES already disclosed in DaikonChemVault: {SMILES}", query.SMILES);
                previewDto.Errors.Add($"The SMILES '{query.SMILES}' is already disclosed in DaikonChemVault under the name: '{vaultMolecule.Name}'.");
                previewDto.IsValid = false;
                return previewDto;
            }

            return previewDto;
        }
    }
}
