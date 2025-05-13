using Daikon.Shared.VM.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;


namespace MLogix.Application.Features.Calculations.Clustering
{
    /*
     * Handler responsible for processing the GenerateClusterCommand.
     * Retrieves HTTP headers from the request context and sends molecule data
     * to the external Molecule API for clustering analysis.
     * Incorporates logging and validation to ensure reliable execution.
     */
    public class GenerateClusterHandler : IRequestHandler<GenerateClusterCommand, List<ClusterVM>>
    {
        private readonly IMoleculeAPI _moleculeAPI;
        private readonly ILogger<GenerateClusterHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor with dependencies injected
        public GenerateClusterHandler(
            IMoleculeAPI moleculeAPI,
            ILogger<GenerateClusterHandler> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _moleculeAPI = moleculeAPI ?? throw new ArgumentNullException(nameof(moleculeAPI));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /*
         * Executes the molecule clustering operation using an external API.
         * Validates inputs and captures headers for contextual use.
         * 
         * @param request - Command containing molecule list
         * @param cancellationToken - Used to cancel the operation if needed
         * @return List of ClusterVM containing the generated clusters
         */
        public async Task<List<ClusterVM>> Handle(GenerateClusterCommand request, CancellationToken cancellationToken)
        {
            // Validate input molecules
            if (request?.Molecules == null || !request.Molecules.Any())
            {
                _logger.LogWarning("Empty or null molecule list received for clustering.");
                throw new ArgumentException("Molecule list cannot be null or empty.", nameof(request.Molecules));
            }

            try
            {
                // Safely get headers from the current request context
                var headers = _httpContextAccessor.HttpContext?.Request?.Headers?
                    .ToDictionary(h => h.Key, h => h.Value.ToString()) 
                    ?? new Dictionary<string, string>();

                // Call the Molecule API to calculate clusters
                var clusterResults = await _moleculeAPI.CalculateClusters(request.Molecules, headers);

                return clusterResults ?? new List<ClusterVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown during molecular cluster generation.");
                throw new ApplicationException("An error occurred while generating molecular clusters.", ex);
            }
        }
    }
}
