using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.Features.Queries.GetMolecule;

namespace MLogix.Application.Features.Queries.ListMolecules
{
    public class ListMoleculesHandler : IRequestHandler<ListMoleculesCommand, List<MoleculeListVM>>
    {
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ListMoleculesHandler> _logger;

        private readonly IMolDbAPIService _molDbAPIService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ListMoleculesHandler(IMoleculeRepository moleculeRepository, IMapper mapper, ILogger<ListMoleculesHandler> logger, 
        IMolDbAPIService molDbAPIService, IHttpContextAccessor httpContextAccessor)
        {
            _moleculeRepository = moleculeRepository;
            _mapper = mapper;
            _logger = logger;
            _molDbAPIService = molDbAPIService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<MoleculeListVM>> Handle(ListMoleculesCommand request, CancellationToken cancellationToken)
        {
            var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());
            var allMolecules = await _moleculeRepository.GetAllMolecules();
            var moleculeVms = _mapper.Map<List<MoleculeListVM>>(allMolecules);

            foreach (var moleculeVm in moleculeVms)
            {
                try
                {
                    var molDbMolecule = await _molDbAPIService.GetMoleculeById(moleculeVm.RegistrationId, headers);
                    _logger.LogInformation("MolDB molecule: {0}", molDbMolecule);

                    if (molDbMolecule != null)
                    {
                        //moleculeVm.Smiles = molDbMolecule.Smiles;
                        //moleculeVm.Smiles = moleculeVm.RequestedSMILES;
                        moleculeVm.SmilesCanonical = molDbMolecule.SmilesCanonical;
                        moleculeVm.MolecularWeight = molDbMolecule.MolecularWeight;
                        moleculeVm.TPSA = molDbMolecule.TPSA;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in ListMoleculesCommand");
                    return moleculeVms;
                }
            }

            return moleculeVms;
        }
    }
}