
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

namespace MLogix.Application.Features.Queries.GetMolecule.BySMILES
{
    public class GetMoleculeBySMILESHandler(IMoleculeRepository moleculeRepository, IMapper mapper, ILogger<GetMoleculeBySMILESHandler> logger,
    IMoleculeAPI iMoleculeAPI, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetMoleculeBySMILESQuery, MoleculeVM>
    {
        private readonly IMoleculeRepository _moleculeRepository = moleculeRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetMoleculeBySMILESHandler> _logger = logger;
        private readonly IMoleculeAPI _iMoleculeAPI = iMoleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<MoleculeVM> Handle(GetMoleculeBySMILESQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var headers = _httpContextAccessor.HttpContext.Request.Headers
                            .ToDictionary(h => h.Key, h => h.Value.ToString());

                var vaultMolecule = await _iMoleculeAPI.GetMoleculeBySMILES(request.SMILES, headers)
                                    ?? throw new ResourceNotFoundException(nameof(GetMoleculeBySMILESHandler), request.SMILES);

                var molecule = await _moleculeRepository.GetMoleculeByRegistrationId(vaultMolecule.Id)
                                    ?? throw new ResourceNotFoundException(nameof(GetMoleculeBySMILESHandler), request.SMILES);


                var moleculeVm = _mapper.Map<MoleculeVM>(molecule, opts => opts.Items["WithMeta"] = request.WithMeta);
                _mapper.Map(vaultMolecule, moleculeVm);

                // fix Ids
                moleculeVm.RegistrationId = vaultMolecule.Id;
                moleculeVm.Id = molecule.Id;

                var trackableEntities = new List<VMMeta> { moleculeVm };
                (moleculeVm.PageLastUpdatedDate, moleculeVm.PageLastUpdatedUser) = VMUpdateTracker.CalculatePageLastUpdated(trackableEntities);

                return moleculeVm;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FindBySMILES");
            }

            throw new ResourceNotFoundException(nameof(GetMoleculeBySMILESHandler), request.SMILES);

        }
    }
}