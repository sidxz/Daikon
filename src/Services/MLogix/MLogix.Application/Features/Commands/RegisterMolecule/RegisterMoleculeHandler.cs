
using AutoMapper;
using CQRS.Core.Handlers;
using Daikon.Events.MLogix;
using MediatR;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.DTOs.MolDbAPI;
using MLogix.Domain.Aggregates;

namespace MLogix.Application.Features.Commands.RegisterMolecule
{
    public class RegisterMoleculeHandler : IRequestHandler<RegisterMoleculeCommand, RegisterMoleculeResponseDTO>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<RegisterMoleculeHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;

        private readonly IEventSourcingHandler<MoleculeAggregate> _moleculeEventSourcingHandler;
        private readonly IMolDbAPIService _molDbAPIService;

        public RegisterMoleculeHandler(IMapper mapper, ILogger<RegisterMoleculeHandler> logger, IMoleculeRepository moleculeRepository, IEventSourcingHandler<MoleculeAggregate> moleculeEventSourcingHandler, IMolDbAPIService molDbAPIService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _moleculeEventSourcingHandler = moleculeEventSourcingHandler ?? throw new ArgumentNullException(nameof(moleculeEventSourcingHandler));
            _molDbAPIService = molDbAPIService ?? throw new ArgumentNullException(nameof(molDbAPIService));
        }

        public async Task<RegisterMoleculeResponseDTO> Handle(RegisterMoleculeCommand request, CancellationToken cancellationToken)
        {
            var registerMoleculeResponseDTO = new RegisterMoleculeResponseDTO();
            // check if smiles is blank throw exception
            if (string.IsNullOrEmpty(request.RequestedSMILES))
            {
                throw new InvalidOperationException("SMILES cannot be blank");
            }

            // register molecule in MolDB
            MoleculeDTO registrationReq;
            try
            {
                registrationReq = await _molDbAPIService.RegisterCompound(request.Name ?? "UnNamed", request.RequestedSMILES);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the molecule in MolDB");
                throw new InvalidOperationException("An error occurred while registering the molecule in MolDB");
            }

            if (registrationReq == null)
            {
                _logger.LogError("An error occurred while registering the molecule in MolDB");
                throw new InvalidOperationException("An error occurred while registering the molecule in MolDB");
            }

            // now check if molecule already exists in our system
            var molecule = await _moleculeRepository.GetMoleculeByRegistrationId(registrationReq.Id);
            if (molecule != null)
            {
                _logger.LogInformation("Molecule already exists in our system, returning existing molecule");
                registerMoleculeResponseDTO.Id = molecule.Id;
                registerMoleculeResponseDTO.RegistrationId = molecule.RegistrationId;
                registerMoleculeResponseDTO.Name = molecule.Name;
                registerMoleculeResponseDTO.Synonyms = molecule.Synonyms;
                registerMoleculeResponseDTO.Ids = molecule.Ids;
                registerMoleculeResponseDTO.WasAlreadyRegistered = true;
                registerMoleculeResponseDTO.Similarity = registrationReq.Similarity;
                registerMoleculeResponseDTO.Smiles = registrationReq.Smiles;
                registerMoleculeResponseDTO.SmilesCanonical = registrationReq.SmilesCanonical;
                registerMoleculeResponseDTO.MolecularWeight = registrationReq.MolecularWeight;
                registerMoleculeResponseDTO.TPSA = registrationReq.TPSA;


                return registerMoleculeResponseDTO;
            }

            // create new molecule

            try
            {
                var newMoleculeCreatedEvent = _mapper.Map<MoleculeCreatedEvent>(request);
                newMoleculeCreatedEvent.RegistrationId = registrationReq.Id;
                newMoleculeCreatedEvent.RequestedSMILES = request.RequestedSMILES;
                newMoleculeCreatedEvent.Synonyms = request.Synonyms != null ? new List<string>(request.Synonyms) : [];
                newMoleculeCreatedEvent.Ids = request.Ids != null ? new Dictionary<string, string>(request.Ids) : [];
                newMoleculeCreatedEvent.SmilesCanonical = registrationReq.SmilesCanonical;

                // create new molecule aggregate
                var aggregate = new MoleculeAggregate(newMoleculeCreatedEvent);
                await _moleculeEventSourcingHandler.SaveAsync(aggregate);

                // return response
                registerMoleculeResponseDTO.Id = request.Id;
                registerMoleculeResponseDTO.RegistrationId = registrationReq.Id;       
                registerMoleculeResponseDTO.Name = request.Name;
                registerMoleculeResponseDTO.Synonyms = request.Synonyms;
                registerMoleculeResponseDTO.Ids = request.Ids;
                registerMoleculeResponseDTO.WasAlreadyRegistered = false;

                registerMoleculeResponseDTO.Similarity = registrationReq.Similarity;
                registerMoleculeResponseDTO.Smiles = registrationReq.Smiles;
                registerMoleculeResponseDTO.SmilesCanonical = registrationReq.SmilesCanonical;
                registerMoleculeResponseDTO.MolecularWeight = registrationReq.MolecularWeight;
                registerMoleculeResponseDTO.TPSA = registrationReq.TPSA;

                return registerMoleculeResponseDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling the molecule in our system");
                throw;
            }

        }
    }
}