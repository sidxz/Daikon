using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Domain.Aggregates;
using MLogix.Application.Utils;
using MLogix.Application.Features.Commands.RegisterMoleculeBatch;
using Daikon.Shared.VM.MLogix;
using MLogix.Application.Features.Commands.PredictNuisance;
using MLogix.Application.BackgroundServices;
using MLogix.Domain.Entities;

namespace MLogix.Application.Features.Commands.UpdateMolecule
{
    public class UpdateMoleculeHandler : IRequestHandler<UpdateMoleculeCommand, UpdateMoleculeResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateMoleculeHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IEventSourcingHandler<MoleculeAggregate> _eventSourcingHandler;
        private readonly IMoleculeAPI _moleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MoleculeUtils _moleculeUtils;
        private readonly INuisanceJobQueue _nuisanceQueue;

        public UpdateMoleculeHandler(
            IMapper mapper,
            ILogger<UpdateMoleculeHandler> logger,
            IMoleculeRepository moleculeRepository,
            IEventSourcingHandler<MoleculeAggregate> moleculeEventSourcingHandler,
            IMoleculeAPI moleculeAPI,
            IHttpContextAccessor httpContextAccessor,
            MoleculeUtils moleculeUtils,
            INuisanceJobQueue nuisanceQueue)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _eventSourcingHandler = moleculeEventSourcingHandler ?? throw new ArgumentNullException(nameof(moleculeEventSourcingHandler));
            _moleculeAPI = moleculeAPI ?? throw new ArgumentNullException(nameof(moleculeAPI));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _moleculeUtils = moleculeUtils ?? throw new ArgumentNullException(nameof(moleculeUtils));
            _nuisanceQueue = nuisanceQueue ?? throw new ArgumentNullException(nameof(nuisanceQueue));
        }

        public async Task<UpdateMoleculeResponseDTO> Handle(UpdateMoleculeCommand request, CancellationToken cancellationToken)
        {
            request.SetUpdateProperties(request.RequestorUserId);
            var headers = GetRequestHeaders();

            var molecule = await _moleculeRepository.GetMoleculeById(request.Id)
                ?? throw new InvalidOperationException("Molecule not found");

            _logger.LogInformation("Update request received for Molecule: {MoleculeId}, Name: {Name}", molecule.Id, molecule.Name);

            return molecule.IsDisclosed()
                ? await HandleDisclosedMoleculeUpdateAsync(request, molecule, headers, cancellationToken)
                : await HandleUndisclosedMoleculeUpdateAsync(request, molecule, headers);
        }

        /* Retrieves headers from the current HTTP context */
        private Dictionary<string, string> GetRequestHeaders()
        {
            return _httpContextAccessor.HttpContext?.Request.Headers
                .ToDictionary(h => h.Key, h => h.Value.ToString())
                ?? throw new InvalidOperationException("HTTP context headers unavailable.");
        }

        /* Handles updates to undisclosed molecules - only name can change */
        private async Task<UpdateMoleculeResponseDTO> HandleUndisclosedMoleculeUpdateAsync(UpdateMoleculeCommand request, Molecule molecule, Dictionary<string, string> headers)
        {
            _logger.LogInformation("Handling undisclosed molecule update...");

            if (!string.IsNullOrWhiteSpace(request.RequestedSMILES))
                throw new InvalidOperationException("SMILES cannot be updated for undisclosed molecules.");
            

            if (molecule.Name == request.Name)
            {
                _logger.LogInformation("No changes detected for undisclosed molecule. Skipping update.");
                return new UpdateMoleculeResponseDTO();
            }

            if (!await _moleculeUtils.IsNameAvailableAsync(request.Name, headers))
                throw new InvalidOperationException($"Molecule name '{request.Name}' is already in use.");

            _logger.LogInformation("Updating name from {OldName} to {NewName}", molecule.Name, request.Name);

            var updateEvent = PrepareMoleculeUpdatedEvent(request, molecule);
            updateEvent.Name = request.Name;

            await SaveMoleculeAggregateAsync(molecule.Id, updateEvent);

            return _mapper.Map<UpdateMoleculeResponseDTO>(updateEvent);
        }

        /* Handles updates to disclosed molecules - name or SMILES can change, but not both */
        private async Task<UpdateMoleculeResponseDTO> HandleDisclosedMoleculeUpdateAsync(UpdateMoleculeCommand request, Molecule molecule, Dictionary<string, string> headers, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling disclosed molecule update...");

            bool nameChanged = molecule.Name != request.Name;
            bool smilesChanged = molecule.RequestedSMILES != request.RequestedSMILES;

            if (nameChanged && smilesChanged)
                throw new InvalidOperationException("Cannot update both name and SMILES in a single update.");

            

            var updateEvent = PrepareMoleculeUpdatedEvent(request, molecule);

            if (nameChanged)
            {
                if (!await _moleculeUtils.IsNameAvailableAsync(request.Name, headers))
                    throw new InvalidOperationException($"Molecule name '{request.Name}' is already in use.");

                var chemVaultMolecule = await _moleculeAPI.GetMoleculeById(molecule.RegistrationId, headers)
                ?? throw new InvalidOperationException("ChemVault molecule not found.");

                updateEvent.Name = request.Name;

                // send request to ChemVault to update name
                var updateVaultMoleculeCmd = new UpdateMoleculeCommand
                {
                    Name = request.Name,
                    RequestedSMILES = chemVaultMolecule.SmilesCanonical,
                    Synonyms = request.Synonyms
                };

                await _moleculeAPI.Update(molecule.RegistrationId, updateVaultMoleculeCmd, headers);


                _logger.LogInformation("Name updated to: {NewName}", request.Name);
            }

            if (smilesChanged)
            {
                await HandleSMILESUpdateAsync(request, molecule, headers, updateEvent);
                await EnqueueNuisancePredictionAsync(request, molecule.Id, cancellationToken);
            }

            await SaveMoleculeAggregateAsync(molecule.Id, updateEvent);

            return _mapper.Map<UpdateMoleculeResponseDTO>(updateEvent);
        }

        /* Checks for SMILES duplication and updates ChemVault */
        private async Task HandleSMILESUpdateAsync(UpdateMoleculeCommand request, Molecule molecule, Dictionary<string, string> headers, MoleculeUpdatedEvent updateEvent)
        {
            var existingMatches = await _moleculeAPI.GetMoleculesBySMILES([request.RequestedSMILES], headers);

            if (existingMatches?.Any() == true)
                throw new InvalidOperationException("SMILES already exists in ChemVault.");

            var oldMolecule = await _moleculeAPI.GetMoleculeById(molecule.RegistrationId, headers)
                ?? throw new InvalidOperationException("Existing ChemVault molecule not found.");

            await _moleculeAPI.Delete(molecule.RegistrationId, headers);

            var registerCommand = new RegisterMoleculeCommandWithRegId
            {
                Id = molecule.RegistrationId,
                Name = request.Name,
                SMILES = request.RequestedSMILES,
                Synonyms = oldMolecule.Synonyms,
                DateCreated = DateTime.UtcNow,
                IsModified = false
            };

            try
            {
                var registered = await _moleculeAPI.RegisterBatch(
                    new List<RegisterMoleculeCommandWithRegId> { registerCommand },
                    previewMode: false,
                    headers: headers);

                if (registered == null || !registered.Any())
                    throw new InvalidOperationException("ChemVault registration failed.");

                updateEvent.RegistrationId = registered[0].Id;
                updateEvent.RequestedSMILES = request.RequestedSMILES;
                _logger.LogInformation("SMILES updated to: {NewSMILES}", request.RequestedSMILES);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register updated SMILES in ChemVault.");
                throw new InvalidOperationException("Failed to register updated SMILES.");
            }
        }

        /* Queues nuisance prediction job for SMILES update */
        private async Task EnqueueNuisancePredictionAsync(UpdateMoleculeCommand request, Guid moleculeId, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var nuisanceCommand = new PredictNuisanceCommand
            {
                NuisanceRequestTuple = new List<DTOs.CageFusion.NuisanceRequestTuple>
                {
                    new() { Id = moleculeId.ToString(), SMILES = request.RequestedSMILES }
                },
                PlotAllAttention = false,
                RequestorUserId = request.RequestorUserId,
                CreatedById = request.RequestorUserId,
                LastModifiedById = request.RequestorUserId,
                DateCreated = now,
                DateModified = now,
                IsModified = false
            };

            var correlationId = Guid.NewGuid().ToString("N");

            try
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
                {
                    var job = new NuisanceJob(nuisanceCommand, correlationId);
                    await _nuisanceQueue.EnqueueAsync(job, cancellationToken);
                    _logger.LogInformation("Nuisance prediction job queued for Molecule: {MoleculeId}", moleculeId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enqueue nuisance prediction job.");
            }
        }

        /* Prepares event object for updating molecule */
        private MoleculeUpdatedEvent PrepareMoleculeUpdatedEvent(UpdateMoleculeCommand request, Molecule molecule)
        {
            var evt = _mapper.Map<MoleculeUpdatedEvent>(request);
            evt.Id = molecule.Id;
            evt.RegistrationId = molecule.RegistrationId;
            evt.Name = molecule.Name;
            evt.RequestedSMILES = molecule.RequestedSMILES;
            return evt;
        }

        /* Loads and saves the updated aggregate state */
        private async Task SaveMoleculeAggregateAsync(Guid moleculeId, MoleculeUpdatedEvent updateEvent)
        {
            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(moleculeId);
                aggregate.UpdateMolecule(updateEvent);
                await _eventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found for Molecule ID: {MoleculeId}", moleculeId);
                throw new ResourceNotFoundException(nameof(MoleculeAggregate), moleculeId);
            }
        }
    }

    /* Extension method to encapsulate disclosure check */
    public static class MoleculeExtensions
    {
        public static bool IsDisclosed(this Molecule molecule) =>
            !string.IsNullOrWhiteSpace(molecule.RequestedSMILES) || molecule.IsStructureDisclosed == true;
    }
}
