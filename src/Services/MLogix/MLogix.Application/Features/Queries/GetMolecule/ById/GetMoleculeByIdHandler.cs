
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

namespace MLogix.Application.Features.Queries.GetMolecule.ById
{
    public class GetMoleculeByIdHandler(IMoleculeRepository moleculeRepository, IMapper mapper,
    ILogger<GetMoleculeByIdHandler> logger, IMoleculeAPI iMoleculeAPI, IHttpContextAccessor httpContextAccessor, IMoleculePredictionRepository moleculePredictionRepository) : IRequestHandler<GetMoleculeByIdQuery, MoleculeVM>
    {
        private readonly IMoleculeRepository _moleculeRepository = moleculeRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetMoleculeByIdHandler> _logger = logger;
        private readonly IMoleculeAPI _iMoleculeAPI = iMoleculeAPI;

        private readonly IMoleculePredictionRepository _moleculePredictionRepository = moleculePredictionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<MoleculeVM> Handle(GetMoleculeByIdQuery request, CancellationToken cancellationToken)
        {

            var molecule = await _moleculeRepository.GetMoleculeById(request.Id)
                            ?? throw new ResourceNotFoundException(nameof(GetMoleculeByIdHandler), request.Id);

            var moleculeVm = _mapper.Map<MoleculeVM>(molecule, opts => opts.Items["WithMeta"] = request.WithMeta);
            

            try
            {
                var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());

                var vaultMolecule = await _iMoleculeAPI.GetMoleculeById(molecule.RegistrationId, headers);
                _logger.LogInformation("Vault molecule: {vaultMolecule.Id}", vaultMolecule.Id);

                _mapper.Map(vaultMolecule, moleculeVm);

                var predictions = await _moleculePredictionRepository.GetByMoleculeIdAsync(molecule.Id);
                if (predictions != null)
                {
                    moleculeVm.Predictions = _mapper.Map<MoleculePredictionsVM>(predictions);
                }
                // Fix Ids
                moleculeVm.RegistrationId = vaultMolecule.Id;
                moleculeVm.Id = molecule.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByIdHandler");
                return moleculeVm;
            }
            var trackableEntities = new List<VMMeta> { moleculeVm };
            (moleculeVm.PageLastUpdatedDate, moleculeVm.PageLastUpdatedUser) = VMUpdateTracker.CalculatePageLastUpdated(trackableEntities);
            
            return moleculeVm;

        }
    }
}