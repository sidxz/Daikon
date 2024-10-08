using AutoMapper;
using CQRS.Core.Extensions;
using CQRS.Core.Handlers;
using Daikon.Events.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.Features.Commands.RegisterMolecule;
using MLogix.Domain.Aggregates;

namespace MLogix.Application.Features.Commands.RegisterMoleculeBatch
{
    public class RegisterMoleculeBatchHandler : IRequestHandler<RegisterMoleculeBatchCommand, List<RegisterMoleculeResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<RegisterMoleculeBatchHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IEventSourcingHandler<MoleculeAggregate> _moleculeEventSourcingHandler;
        private readonly IMoleculeAPI _iMoleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterMoleculeBatchHandler(
            IMapper mapper,
            ILogger<RegisterMoleculeBatchHandler> logger,
            IMoleculeRepository moleculeRepository,
            IEventSourcingHandler<MoleculeAggregate> moleculeEventSourcingHandler,
            IMoleculeAPI iMoleculeAPI,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _moleculeEventSourcingHandler = moleculeEventSourcingHandler ?? throw new ArgumentNullException(nameof(moleculeEventSourcingHandler));
            _iMoleculeAPI = iMoleculeAPI ?? throw new ArgumentNullException(nameof(iMoleculeAPI));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<List<RegisterMoleculeResponseDTO>> Handle(RegisterMoleculeBatchCommand request, CancellationToken cancellationToken)
        {
            var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());

            var allResponses = new List<RegisterMoleculeResponseDTO>();

            // deep copy the request
            var originalRequest = _mapper.Map<RegisterMoleculeBatchCommand>(request);

            // Split the list of commands into batches of 500
            var batches = request.Commands.Batch(500);

            foreach (var batch in batches)
            {
                try
                {
                    // check if Id and RegistrationId are set, if not create new Guid
                    foreach (var command in batch)
                    {
                        if (command.Id == Guid.Empty)
                        {
                            command.Id = Guid.NewGuid();
                        }

                        if (command.RegistrationId == Guid.Empty)
                        {
                            command.RegistrationId = Guid.NewGuid();
                        }

                        // temporarily set Id to be the same as RegistrationId for vault registration

                        command.Id = command.RegistrationId;
                    }

                    // Register batch of molecules in MolDB
                    var registrationResponses = await _iMoleculeAPI.RegisterBatch(batch, headers);

                    foreach (var registrationReq in registrationResponses)
                    {
                        // Check if molecule already exists in our system
                        var molecule = await _moleculeRepository.GetMoleculeByRegistrationId(registrationReq.Id);
                        RegisterMoleculeResponseDTO registerMoleculeResponseDTO;

                        if (molecule != null)
                        {
                            _logger.LogInformation("Molecule {Name} already exists in our system, returning existing molecule", registrationReq.Name);
                            registerMoleculeResponseDTO = _mapper.Map<RegisterMoleculeResponseDTO>(registrationReq);
                            registerMoleculeResponseDTO.WasAlreadyRegistered = true;
                            registerMoleculeResponseDTO.RegistrationId = registrationReq.Id;
                            registerMoleculeResponseDTO.Id = molecule.Id;
                        }
                        else
                        {
                            // Create new molecule
                            _logger.LogInformation("Creating new molecule {Name} in our system", registrationReq.Name);
                            var newMoleculeCreatedEvent = _mapper.Map<MoleculeCreatedEvent>(registrationReq);
                            newMoleculeCreatedEvent.Id = originalRequest.Commands.First(x => x.RegistrationId == registrationReq.Id).Id;
                            newMoleculeCreatedEvent.RegistrationId = registrationReq.Id;
                            newMoleculeCreatedEvent.RequestedSMILES = request.Commands.First(x => x.RegistrationId == registrationReq.Id).SMILES;
                            newMoleculeCreatedEvent.SmilesCanonical = registrationReq.SmilesCanonical;

                            // Create new molecule aggregate
                            var aggregate = new MoleculeAggregate(newMoleculeCreatedEvent);
                            await _moleculeEventSourcingHandler.SaveAsync(aggregate);

                            // Return response
                            registerMoleculeResponseDTO = _mapper.Map<RegisterMoleculeResponseDTO>(registrationReq);
                            registerMoleculeResponseDTO.WasAlreadyRegistered = false;
                            registerMoleculeResponseDTO.RegistrationId = registrationReq.Id;
                            registerMoleculeResponseDTO.Id = newMoleculeCreatedEvent.Id;
                        }

                        allResponses.Add(registerMoleculeResponseDTO);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing a batch of molecules");
                    throw new InvalidOperationException("An error occurred while processing a batch of molecules");
                }
            }

            return allResponses;
        }
    }
}
