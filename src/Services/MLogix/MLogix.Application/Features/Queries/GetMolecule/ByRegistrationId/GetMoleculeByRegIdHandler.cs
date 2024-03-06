
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure;
using MLogix.Application.Contracts.Persistence;

namespace MLogix.Application.Features.Queries.GetMolecule.ByRegistrationId
{
    public class GetMoleculeByRegIdHandler : IRequestHandler<GetMoleculeByRegIdQuery, MoleculeVM>
    {
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetMoleculeByRegIdHandler> _logger;

        private readonly IMolDbAPIService _molDbAPIService;


        public GetMoleculeByRegIdHandler(IMoleculeRepository moleculeRepository, IMapper mapper, ILogger<GetMoleculeByRegIdHandler> logger, IMolDbAPIService molDbAPIService)
        {
            _moleculeRepository = moleculeRepository;
            _mapper = mapper;
            _logger = logger;
            _molDbAPIService = molDbAPIService;
        }
        public async Task<MoleculeVM> Handle(GetMoleculeByRegIdQuery request, CancellationToken cancellationToken)
        {

            var molecule = await _moleculeRepository.GetMoleculeByRegistrationId(request.RegistrationId);

            if (molecule == null)
            {
                throw new ResourceNotFoundException(nameof(GetMoleculeByRegIdHandler), request.RegistrationId);
            }

            var moleculeVm = _mapper.Map<MoleculeVM>(molecule, opts => opts.Items["WithMeta"] = request.WithMeta);

            try
            {
                var molDbMolecule = await _molDbAPIService.GetMoleculeById(molecule.RegistrationId);

                if (molDbMolecule != null)
                {
                    moleculeVm.Smiles = molDbMolecule.Smiles;
                    moleculeVm.SmilesCanonical = molDbMolecule.SmilesCanonical;
                    moleculeVm.MolecularWeight = molDbMolecule.MolecularWeight;
                    moleculeVm.TPSA = molDbMolecule.TPSA;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FindExact");
                return moleculeVm;
            }
            return moleculeVm;

        }
    }
}