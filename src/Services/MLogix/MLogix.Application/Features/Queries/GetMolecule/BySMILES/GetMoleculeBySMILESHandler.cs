
using Amazon.Auth.AccessControlPolicy;
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure;
using MLogix.Application.Contracts.Persistence;

namespace MLogix.Application.Features.Queries.GetMolecule.BySMILES
{
    public class GetMoleculeBySMILESHandler : IRequestHandler<GetMoleculeBySMILESQuery, MoleculeVM>
    {
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetMoleculeBySMILESHandler> _logger;

        private readonly IMolDbAPIService _molDbAPIService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public GetMoleculeBySMILESHandler(IMoleculeRepository moleculeRepository, IMapper mapper, ILogger<GetMoleculeBySMILESHandler> logger,
        IMolDbAPIService molDbAPIService, IHttpContextAccessor httpContextAccessor)
        {
            _moleculeRepository = moleculeRepository;
            _mapper = mapper;
            _logger = logger;
            _molDbAPIService = molDbAPIService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<MoleculeVM> Handle(GetMoleculeBySMILESQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var headers = _httpContextAccessor.HttpContext.Request.Headers
                            .ToDictionary(h => h.Key, h => h.Value.ToString());
                var molDbMolecule = await _molDbAPIService.GetMoleculeBySMILES(request.SMILES, headers);

                if (molDbMolecule == null)
                {
                    throw new ResourceNotFoundException(nameof(GetMoleculeBySMILESHandler), request.SMILES);
                }
                var molecule = await _moleculeRepository.GetMoleculeByRegistrationId(molDbMolecule.Id);
                if (molecule == null)
                {
                    throw new ResourceNotFoundException(nameof(GetMoleculeBySMILESHandler), request.SMILES);
                }

                var moleculeVm = _mapper.Map<MoleculeVM>(molecule, opts => opts.Items["WithMeta"] = request.WithMeta);
                moleculeVm.Smiles = molDbMolecule.Smiles;
                moleculeVm.SmilesCanonical = molDbMolecule.SmilesCanonical;
                moleculeVm.MolecularWeight = molDbMolecule.MolecularWeight;
                moleculeVm.TPSA = molDbMolecule.TPSA;
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