
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure;
using MLogix.Application.Contracts.Persistence;

namespace MLogix.Application.Features.Queries.GetMolecule.ById
{
    public class GetMoleculeByIdHandler : IRequestHandler<GetMoleculeByIdQuery, MoleculeVM>
    {
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetMoleculeByIdHandler> _logger;

        private readonly IMolDbAPIService _molDbAPIService;


        public GetMoleculeByIdHandler(IMoleculeRepository moleculeRepository, IMapper mapper, ILogger<GetMoleculeByIdHandler> logger, IMolDbAPIService molDbAPIService)
        {
            _moleculeRepository = moleculeRepository;
            _mapper = mapper;
            _logger = logger;
            _molDbAPIService = molDbAPIService;
        }
        public async Task<MoleculeVM> Handle(GetMoleculeByIdQuery request, CancellationToken cancellationToken)
        {

            var molecule = await _moleculeRepository.GetMoleculeById(request.Id);

            if (molecule == null)
            {
                throw new ResourceNotFoundException(nameof(GetMoleculeByIdHandler), request.Id);
            }

            var moleculeVm = _mapper.Map<MoleculeVM>(molecule, opts => opts.Items["WithMeta"] = request.WithMeta);

            try
            {
                var molDbMolecule = await _molDbAPIService.GetMoleculeById(molecule.RegistrationId);
                _logger.LogInformation("MolDB molecule: {0}", molDbMolecule);

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