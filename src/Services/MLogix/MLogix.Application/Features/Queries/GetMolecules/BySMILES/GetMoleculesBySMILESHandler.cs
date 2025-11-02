
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.Shared.VM.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Domain.Entities;

namespace MLogix.Application.Features.Queries.GetMolecules.BySMILES
{
    public class GetMoleculesBySMILESHandler(IMoleculeRepository moleculeRepository, IMapper mapper, ILogger<GetMoleculesBySMILESHandler> logger,
    IMoleculeAPI iMoleculeAPI, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetMoleculesBySMILESQuery, List<MoleculeVM>>
    {
        private readonly IMoleculeRepository _moleculeRepository = moleculeRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetMoleculesBySMILESHandler> _logger = logger;
        private readonly IMoleculeAPI _iMoleculeAPI = iMoleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<List<MoleculeVM>> Handle(GetMoleculesBySMILESQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var headers = _httpContextAccessor.HttpContext.Request.Headers
                            .ToDictionary(h => h.Key, h => h.Value.ToString());




                List<MoleculeBase> vaultMolecules = await _iMoleculeAPI.GetMoleculesBySMILES(request.SMILES, headers)
                                    ?? throw new ResourceNotFoundException(nameof(GetMoleculesBySMILESHandler), "");

                if (vaultMolecules.Count == 0)
                {
                    return [];
                }

                List<Molecule> molecules = await _moleculeRepository.GetMoleculesByRegistrationId(vaultMolecules.Select(vm => vm.Id).ToList())
                                    ?? throw new ResourceNotFoundException(nameof(GetMoleculesBySMILESHandler), "");


                List<MoleculeVM> moleculeVms = _mapper.Map<List<MoleculeVM>>(molecules, opts => opts.Items["WithMeta"] = request.WithMeta);
                _mapper.Map(vaultMolecules, moleculeVms);

                // fix Ids by matching RegistrationId
                var vaultMoleculeDict = vaultMolecules.ToDictionary(vm => vm.Id);
                var moleculeDict = molecules.ToDictionary(m => m.RegistrationId);

                foreach (var moleculeVm in moleculeVms)
                {
                    if (vaultMoleculeDict.TryGetValue(moleculeVm.RegistrationId, out var vaultMolecule))
                    {
                        moleculeVm.RegistrationId = vaultMolecule.Id;
                    }
                    if (moleculeDict.TryGetValue(moleculeVm.RegistrationId, out var molecule))
                    {
                        moleculeVm.Id = molecule.Id;
                    }
                }


                return moleculeVms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FindBySMILES");
            }
            
            throw new ResourceNotFoundException(nameof(GetMoleculesBySMILESHandler), "");

        }
    }
}