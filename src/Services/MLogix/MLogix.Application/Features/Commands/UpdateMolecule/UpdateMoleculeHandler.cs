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
using MLogix.Application.Features.Commands.RegisterMoleculeBatch;
using MLogix.Application.Features.Commands.PredictNuisance;
using MLogix.Application.Utils;
using MLogix.Application.BackgroundServices;
using MLogix.Domain.Aggregates;
using MLogix.Domain.Entities;
using Daikon.Shared.VM.MLogix;

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

            var molecule = await _moleculeRepository.GetMoleculeById(request.Id);
            if (molecule is null)
            {
                // Preview mode should not throw.
                return request.PreviewMode
                    ? PreviewFailure("PREVIEW_FAILED_NOT_FOUND", "Molecule not found.")
                    : throw new InvalidOperationException("Molecule not found");
            }

            _logger.LogInformation(
                "Update request received for Molecule: {MoleculeId}, Name: {Name}, PreviewMode: {PreviewMode}",
                molecule.Id, molecule.Name, request.PreviewMode);

            return molecule.IsDisclosed()
                ? await HandleDisclosedMoleculeUpdateAsync(request, molecule, headers, cancellationToken)
                : await HandleUndisclosedMoleculeUpdateAsync(request, molecule, headers);
        }

        /*
         * Retrieves headers from the current HTTP context.
         * NOTE: If your code can run outside an HTTP request (background jobs),
         * you may want to allow an empty header dictionary instead of failing.
         */
        private Dictionary<string, string> GetRequestHeaders()
        {
            return _httpContextAccessor.HttpContext?.Request.Headers
                       .ToDictionary(h => h.Key, h => h.Value.ToString())
                   ?? throw new InvalidOperationException("HTTP context headers unavailable.");
        }

        /*
         * ----------------------------------------
         * PREVIEW MODE CONTRACT (enforced here)
         * ----------------------------------------
         * - If PreviewMode == true:
         *     - Do NOT mutate aggregates (event store)
         *     - Do NOT call ChemVault mutation endpoints (Update/Delete/Register)
         *     - Do NOT enqueue nuisance jobs
         *     - Do NOT throw exceptions; return PreviewStatus that explains success/failure/no-op
         * - If PreviewMode == false:
         *     - Perform mutations
         *     - Throw exceptions as before
         */

        private async Task<UpdateMoleculeResponseDTO> HandleUndisclosedMoleculeUpdateAsync(
            UpdateMoleculeCommand request,
            Molecule molecule,
            Dictionary<string, string> headers)
        {
            _logger.LogInformation("Handling undisclosed molecule update...");

            if (!string.IsNullOrWhiteSpace(request.RequestedSMILES))
            {
                return request.PreviewMode
                    ? PreviewFailure("PREVIEW_FAILED_INVALID", "SMILES cannot be updated for undisclosed molecules.")
                    : throw new InvalidOperationException("SMILES cannot be updated for undisclosed molecules.");
            }

            if (string.Equals(molecule.Name, request.Name, StringComparison.Ordinal))
            {
                _logger.LogInformation("No changes detected for undisclosed molecule. Skipping update.");
                return PreviewNoop(request.PreviewMode, "NO_CHANGES_UNDISCLOSED");
            }

            var isNameAvailable = await _moleculeUtils.IsNameAvailableAsync(request.Name, headers);
            if (!isNameAvailable)
            {
                return request.PreviewMode
                    ? PreviewFailure("PREVIEW_FAILED_CONFLICT", $"Molecule name '{request.Name}' is already in use.")
                    : throw new InvalidOperationException($"Molecule name '{request.Name}' is already in use.");
            }

            _logger.LogInformation("Updating name from {OldName} to {NewName}", molecule.Name, request.Name);

            var updateEvent = PrepareMoleculeUpdatedEvent(request, molecule);
            updateEvent.Name = request.Name;

            await SaveMoleculeAggregateAsync(molecule.Id, updateEvent, request.PreviewMode);

            var response = _mapper.Map<UpdateMoleculeResponseDTO>(updateEvent);
            response.PreviewStatus = request.PreviewMode
                ? $"PREVIEW_OK_WILL_UPDATE_UNDISCLOSED_NAME:{request.Name}"
                : "UPDATED_UNDISCLOSED_NAME";

            return response;
        }

        private async Task<UpdateMoleculeResponseDTO> HandleDisclosedMoleculeUpdateAsync(
            UpdateMoleculeCommand request,
            Molecule molecule,
            Dictionary<string, string> headers,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling disclosed molecule update...");

            var chemVaultMolecule = await _moleculeAPI.GetMoleculeById(molecule.RegistrationId, headers);
            if (chemVaultMolecule is null)
            {
                return request.PreviewMode
                    ? PreviewFailure("PREVIEW_FAILED_NOT_FOUND", "ChemVault molecule not found.")
                    : throw new InvalidOperationException("ChemVault molecule not found.");
            }

            var nameChanged = !string.Equals(molecule.Name, request.Name, StringComparison.Ordinal);
            var smilesChanged = !string.Equals(molecule.RequestedSMILES, request.RequestedSMILES, StringComparison.Ordinal);
            var existingSynonyms = StringUtilities.ExtractSynonyms(chemVaultMolecule.Synonyms);
            var requestedSynonyms = StringUtilities.ExtractSynonyms(request.Synonyms ?? string.Empty);
            var synonymsChanged = !existingSynonyms.SequenceEqual(requestedSynonyms);
            var nameOrSynonymsChanged = nameChanged || synonymsChanged;



            if (nameOrSynonymsChanged && smilesChanged)
            {
                return request.PreviewMode
                    ? PreviewFailure("PREVIEW_FAILED_INVALID", "Cannot update both name/synonyms and SMILES in a single update.")
                    : throw new InvalidOperationException("Cannot update both name/synonyms and SMILES in a single update.");
            }

            var updateEvent = PrepareMoleculeUpdatedEvent(request, molecule);

            // --- Name update flow (read-only checks allowed in preview; mutations forbidden in preview)
            if (nameChanged)
            {
                var isNameAvailable = await _moleculeUtils.IsNameAvailableAsync(request.Name, headers);
                if (!isNameAvailable)
                {
                    return request.PreviewMode
                        ? PreviewFailure("PREVIEW_FAILED_CONFLICT", $"Molecule name '{request.Name}' is already in use.")
                        : throw new InvalidOperationException($"Molecule name '{request.Name}' is already in use.");
                }



                updateEvent.Name = request.Name;

                // Only mutate ChemVault if not preview.
                if (!request.PreviewMode)
                {
                    var updateVaultMoleculeCmd = new UpdateMoleculeCommand
                    {
                        Name = request.Name,
                        RequestedSMILES = chemVaultMolecule.SmilesCanonical,
                        Synonyms = request.Synonyms
                    };

                    await _moleculeAPI.Update(molecule.RegistrationId, updateVaultMoleculeCmd, headers);
                }

                _logger.LogInformation("Name {Action}: {NewName}",
                    request.PreviewMode ? "will be updated (preview)" : "updated",
                    request.Name);
            }

            // synonyms update flow (read-only checks allowed in preview; mutations forbidden in preview)
            if (synonymsChanged)
            {
                // lets loop through the new synonyms and check if they are available, fail if any one is not
                var newSynonyms = requestedSynonyms.Except(existingSynonyms, StringComparer.OrdinalIgnoreCase);
                foreach (var synonym in newSynonyms)
                {
                    var isSynonymAvailable = await _moleculeUtils.IsNameAvailableAsync(synonym, headers);
                    if (!isSynonymAvailable)
                    {
                        return request.PreviewMode
                            ? PreviewFailure("PREVIEW_FAILED_CONFLICT", $"Molecule synonym '{synonym}' is already in use.")
                            : throw new InvalidOperationException($"Molecule synonym '{synonym}' is already in use.");
                    }
                }
                // There is no need to set anything in the event for synonyms, as they are not stored in our aggregate
                // we just need to update ChemVault

                // Only mutate ChemVault if not preview.
                if (!request.PreviewMode)
                {
                    var updateVaultMoleculeCmd = new UpdateMoleculeCommand
                    {
                        Name = chemVaultMolecule.Name,
                        RequestedSMILES = chemVaultMolecule.SmilesCanonical,
                        Synonyms = request.Synonyms
                    };

                    await _moleculeAPI.Update(molecule.RegistrationId, updateVaultMoleculeCmd, headers);
                }
                _logger.LogInformation("Synonyms {Action}",
                    request.PreviewMode ? "will be updated (preview)" : "updated");
            }



            // --- SMILES update flow (read-only checks allowed in preview; mutations forbidden in preview)
            if (smilesChanged)
            {
                var smilesResult = await HandleSMILESUpdateAsync(request, molecule, headers, updateEvent);
                if (!smilesResult.Success)
                {
                    // HandleSMILESUpdateAsync already returns a preview-friendly outcome for preview mode.
                    return request.PreviewMode
                        ? PreviewFailure("PREVIEW_FAILED_SMILES", smilesResult.Message)
                        : throw new InvalidOperationException(smilesResult.Message);
                }

                // Only enqueue nuisance job if not preview (also guarded inside).
                await EnqueueNuisancePredictionAsync(request, molecule.Id, cancellationToken);
            }

            await SaveMoleculeAggregateAsync(molecule.Id, updateEvent, request.PreviewMode);

            var response = _mapper.Map<UpdateMoleculeResponseDTO>(updateEvent);
            response.PreviewStatus = request.PreviewMode
                ? BuildPreviewStatus(nameChanged, smilesChanged, updateEvent)
                : BuildNonPreviewStatus(nameChanged, smilesChanged);

            return response;
        }

        private static string BuildPreviewStatus(bool nameChanged, bool smilesChanged, MoleculeUpdatedEvent updateEvent)
        {
            if (nameChanged) return $"PREVIEW_OK_WILL_UPDATE_NAME:{updateEvent.Name}";
            if (smilesChanged) return $"PREVIEW_OK_WILL_UPDATE_SMILES:{updateEvent.RequestedSMILES}";
            return "PREVIEW_OK";
        }

        private static string BuildNonPreviewStatus(bool nameChanged, bool smilesChanged)
        {
            if (nameChanged) return "UPDATED_NAME";
            if (smilesChanged) return "UPDATED_SMILES";
            return "UPDATED";
        }

        private sealed record SmilesUpdateResult(bool Success, string Message);

        /*
         * Checks for SMILES duplication and (only when not preview) updates ChemVault via delete + register.
         * - Preview: performs read checks, sets event fields, returns success/failure WITHOUT mutating ChemVault.
         * - Non-preview: performs full mutation; exceptions propagate.
         */
        private async Task<SmilesUpdateResult> HandleSMILESUpdateAsync(
            UpdateMoleculeCommand request,
            Molecule molecule,
            Dictionary<string, string> headers,
            MoleculeUpdatedEvent updateEvent)
        {
            try
            {
                // Read-only duplication check is allowed in preview.
                var existingMatches = await _moleculeAPI.GetMoleculesBySMILES([request.RequestedSMILES], headers);
                if (existingMatches?.Any() == true)
                {
                    return new SmilesUpdateResult(false, "SMILES already exists in ChemVault.");
                }

                // Read-only fetch old molecule allowed in preview.
                var oldMolecule = await _moleculeAPI.GetMoleculeById(molecule.RegistrationId, headers);
                if (oldMolecule is null)
                {
                    return new SmilesUpdateResult(false, "Existing ChemVault molecule not found.");
                }

                // In preview mode, do NOT mutate ChemVault (no delete/register).
                if (request.PreviewMode)
                {
                    updateEvent.RequestedSMILES = request.RequestedSMILES;
                    // RegistrationId would remain the same in the event store preview; real flow may re-register.
                    updateEvent.RegistrationId = molecule.RegistrationId;

                    _logger.LogInformation("Preview mode - SMILES would be updated to: {NewSmiles}", request.RequestedSMILES);
                    return new SmilesUpdateResult(true, $"SMILES will be updated to '{request.RequestedSMILES}' (preview).");
                }

                // Non-preview: perform actual mutation.
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

                var registered = await _moleculeAPI.RegisterBatch(
                    new List<RegisterMoleculeCommandWithRegId> { registerCommand },
                    previewMode: false,
                    headers: headers);

                if (registered is null || !registered.Any())
                {
                    return new SmilesUpdateResult(false, "ChemVault registration failed.");
                }

                updateEvent.RegistrationId = registered[0].Id;
                updateEvent.RequestedSMILES = request.RequestedSMILES;

                _logger.LogInformation("SMILES updated to: {NewSMILES}", request.RequestedSMILES);
                return new SmilesUpdateResult(true, "SMILES updated.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMILES update flow failed (PreviewMode: {PreviewMode}).", request.PreviewMode);

                // Preview mode must not throw.
                if (request.PreviewMode)
                {
                    return new SmilesUpdateResult(false, "SMILES update preview failed due to an internal error.");
                }

                // Non-preview: throw, but do not leak sensitive details.
                throw new InvalidOperationException("Failed to register updated SMILES.");
            }
        }

        /*
         * Queues nuisance prediction job for SMILES update.
         * Preview mode: must do nothing and must not throw.
         */
        private async Task EnqueueNuisancePredictionAsync(UpdateMoleculeCommand request, Guid moleculeId, CancellationToken cancellationToken)
        {
            if (request.PreviewMode)
            {
                _logger.LogInformation("Preview mode enabled - skipping nuisance prediction enqueue.");
                return;
            }

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
                // Non-critical path; log only.
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

        /*
         * Loads and saves the updated aggregate state.
         * Preview mode: MUST NOT mutate aggregate state (enforced here).
         */
        private async Task SaveMoleculeAggregateAsync(Guid moleculeId, MoleculeUpdatedEvent updateEvent, bool previewMode)
        {
            if (previewMode)
            {
                _logger.LogInformation("Preview mode enabled - skipping aggregate save for Molecule ID: {MoleculeId}", moleculeId);
                return;
            }

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

        // -------- Preview response helpers --------

        private static UpdateMoleculeResponseDTO PreviewFailure(string status, string reason)
        {
            // Keep contract simple: a status that is machine-readable but also contains a human hint.
            return new UpdateMoleculeResponseDTO
            {
                PreviewStatus = $"{status}:{reason}"
            };
        }

        private static UpdateMoleculeResponseDTO PreviewNoop(bool previewMode, string status)
        {
            return new UpdateMoleculeResponseDTO
            {
                PreviewStatus = previewMode ? $"PREVIEW_NOOP:{status}" : status
            };
        }
    }

    /* Extension method to encapsulate disclosure check */
    public static class MoleculeExtensions
    {
        public static bool IsDisclosed(this Molecule molecule) =>
            !string.IsNullOrWhiteSpace(molecule.RequestedSMILES) || molecule.IsStructureDisclosed == true;
    }
}
