using AutoMapper;
using Daikon.EventStore.Handlers;
using Daikon.Events.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;
using MLogix.Domain.Aggregates;

namespace MLogix.Application.Features.Commands.RegisterUndisclosed
{
    public class RegisterUndisclosedHandler : IRequestHandler<RegisterUndisclosedCommand, RegisterUndisclosedDTO>
    {
        /*
        * Handler responsible for registering undisclosed molecules.
        * Implements validation, uniqueness checks, and conditional registration logic.
     */
        private readonly IMapper _mapper;
        private readonly ILogger<RegisterUndisclosedHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IEventSourcingHandler<MoleculeAggregate> _moleculeEventSourcingHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMoleculeAPI _moleculeApi;

        public RegisterUndisclosedHandler(
            IMapper mapper,
            ILogger<RegisterUndisclosedHandler> logger,
            IMoleculeRepository moleculeRepository,
            IEventSourcingHandler<MoleculeAggregate> moleculeEventSourcingHandler,
            IHttpContextAccessor httpContextAccessor,
            IMoleculeAPI moleculeApi)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _moleculeEventSourcingHandler = moleculeEventSourcingHandler ?? throw new ArgumentNullException(nameof(moleculeEventSourcingHandler));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _moleculeApi = moleculeApi ?? throw new ArgumentNullException(nameof(moleculeApi));
        }

        public async Task<RegisterUndisclosedDTO> Handle(RegisterUndisclosedCommand request, CancellationToken cancellationToken)
        {

            /*
             * Validate input
             */
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Molecule name is required.", nameof(request.Name));
            }

            /*
             * Retrieve headers for external API calls
             */
            var headers = _httpContextAccessor.HttpContext?.Request?.Headers?
                .ToDictionary(h => h.Key, h => h.Value.ToString()) ?? new Dictionary<string, string>();


            request.SetCreateProperties(request.RequestorUserId);

            request.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;

            var responseDto = new RegisterUndisclosedDTO
            {
                Id = request.Id,
                Name = request.Name,
                WasAlreadyRegistered = false
            };

            if (request.ChemVaultCheck)
            {
                var existingMoleculeInChemVault = await _moleculeApi.FindByNamesOrSynonymsExact([request.Name], headers);

                if (existingMoleculeInChemVault?.Count > 0)
                {
                    var existingRegId = existingMoleculeInChemVault[0].Id;
                    throw new InvalidOperationException($"A disclosed molecule with this name (or synonym) already exists in ChemVault with Registration ID: {existingRegId}");
                }
            }

            /*
             * Check if the molecule already exists in MLogix
             */

            var existingMoleculeInMLogix = await _moleculeRepository.GetByName(request.Name);
            if (existingMoleculeInMLogix != null)
            {
                _logger.LogInformation("Molecule '{Name}' already registered in MLogix with Id {Id}.", request.Name, existingMoleculeInMLogix.Id);

                responseDto.Id = existingMoleculeInMLogix.Id;
                responseDto.RegistrationId = existingMoleculeInMLogix.RegistrationId;
                responseDto.WasAlreadyRegistered = true;
                responseDto.PreviewMessage = "Already exists as undisclosed.";
                responseDto.PreviewStatus = "DUPLICATE_UNDISCLOSED";
                return responseDto;
            }

            /*
             * If in preview mode, simulate the registration and return
             */
            if (request.PreviewMode)
            {
                _logger.LogInformation("Preview mode enabled: No molecule will be registered.");
                responseDto.RegistrationId = Guid.NewGuid();
                responseDto.PreviewMessage = "Would register new undisclosed molecule.";
                responseDto.PreviewStatus = "REGISTER_UNDISCLOSED";
                return responseDto;
            }

            /*
            * Proceed with actual registration
            */
            try
            {
                _logger.LogInformation("Registering new undisclosed molecule '{Name}' in MLogix.", request.Name);
                var newMoleculeCreatedEvent = _mapper.Map<MoleculeCreatedEvent>(request);
                newMoleculeCreatedEvent.Id = request.Id;
                newMoleculeCreatedEvent.RegistrationId = Guid.NewGuid();


                // create new molecule aggregate
                var newAggregate = new MoleculeAggregate(newMoleculeCreatedEvent);
                await _moleculeEventSourcingHandler.SaveAsync(newAggregate);

                responseDto.RegistrationId = newMoleculeCreatedEvent.RegistrationId;
                responseDto.PreviewMessage = "Would register new undisclosed molecule.";
                responseDto.PreviewStatus = "REGISTER_UNDISCLOSED";

                return responseDto;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering undisclosed molecule '{Name}'", request.Name);
                throw new ApplicationException("An unexpected error occurred while registering the molecule. Please contact support.", ex);
            }
        }
    }

}