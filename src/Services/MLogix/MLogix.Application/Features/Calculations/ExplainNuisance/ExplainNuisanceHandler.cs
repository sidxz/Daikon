using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.CageFusion;
using MLogix.Application.DTOs.CageFusion;

namespace MLogix.Application.Features.Calculations.ExplainNuisance
{
    public class ExplainNuisanceHandler : IRequestHandler<ExplainNuisanceCommand, NuisanceResponseDTO>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<ExplainNuisanceHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INuisanceAPI _nuisanceAPI;

        public ExplainNuisanceHandler(IMapper mapper, ILogger<ExplainNuisanceHandler> logger, IHttpContextAccessor httpContextAccessor, INuisanceAPI nuisanceAPI)
        {
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _nuisanceAPI = nuisanceAPI;
        }

        public async Task<NuisanceResponseDTO> Handle(ExplainNuisanceCommand request, CancellationToken cancellationToken)
        {
            request.SetCreateProperties(request.RequestorUserId);

            if (request.NuisanceRequestTuple == null)
            {
                throw new ArgumentException("NuisanceRequestTuple cannot be null or empty");
            }

            NuisanceRequestDTO nuisanceRequestDTO = new NuisanceRequestDTO
            {
                PlotAllAttention = request.PlotAllAttention,
                Items = [request.NuisanceRequestTuple]
            };

            NuisanceResponseDTO result;
            try
            {
                result = await _nuisanceAPI.GetNuisancePredictionsAsync(nuisanceRequestDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting nuisance predictions.");
                throw;
            }

            return result;
        }
    }
}