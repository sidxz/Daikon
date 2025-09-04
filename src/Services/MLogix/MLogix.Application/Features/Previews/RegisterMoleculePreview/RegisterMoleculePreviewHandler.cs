
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;

namespace MLogix.Application.Features.Previews.RegisterMoleculePreview
{
    public class RegisterMoleculePreviewHandler : IRequestHandler<RegisterMoleculePreviewQuery, List<RegisterMoleculePreviewDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<RegisterMoleculePreviewHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IMoleculeAPI _moleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterMoleculePreviewHandler(
            IMapper mapper,
            ILogger<RegisterMoleculePreviewHandler> logger,
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

        public async Task<List<RegisterMoleculePreviewDTO>> Handle(RegisterMoleculePreviewQuery request, CancellationToken cancellationToken)
        {
            if (request.Queries == null || request.Queries.Count == 0)
            {
                throw new ArgumentException("At least one query must be provided.");
            }

            var results = new List<RegisterMoleculePreviewDTO>();
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

        private async Task<RegisterMoleculePreviewDTO> ProcessQueryAsync(QueryItem query, Dictionary<string, string> headers, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query.MoleculeName) || string.IsNullOrWhiteSpace(query.SMILES))
            {
                _logger.LogWarning("Skipping query due to missing required fields. MoleculeName: {MoleculeName}, SMILES: {SMILES}", query.MoleculeName, query.SMILES);
                return null;
            }

            var previewDto = new RegisterMoleculePreviewDTO
            {
                MoleculeName = query.MoleculeName,
                SMILES = query.SMILES,
                Errors = [],
                IsValid = true,
                IsSMILERegistered = false
            };

            // Check if the molecule exists in MLOGIX
            var existingMolecule = await _moleculeRepository.GetByName(query.MoleculeName);
            if (existingMolecule != null)
            {
                _logger.LogWarning("Molecule found in MLOGIX: {MoleculeName}", query.MoleculeName);
                previewDto.Errors.Add($"The Molecule MoleculeName '{query.MoleculeName}' is already registered.");
                previewDto.IsValid = false;
                return previewDto;
            }

            // check if SMILES is provdided in query and is not blank or null
            if (!string.IsNullOrWhiteSpace(query.SMILES))
            {
                var vaultMolecule = await _moleculeAPI.GetMoleculeBySMILES(query.SMILES, headers);
                if (vaultMolecule != null)
                {
                    _logger.LogWarning("SMILES already registered in DaikonChemVault: {SMILES}", query.SMILES);
                    previewDto.IsSMILERegistered = true;
                    previewDto.Errors.Add($"The SMILES '{query.SMILES}' is already registered in DaikonChemVault.");
                    previewDto.IsValid = false;
                    return previewDto;
                }
            }
            return previewDto;
        }
    }
}
