using AutoMapper;
using CQRS.Core.Extensions;
using Daikon.EventStore.Handlers;
using Daikon.Events.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.Features.Commands.RegisterMolecule;
using MLogix.Application.Features.Commands.RegisterUndisclosed;
using MLogix.Domain.Aggregates;
using MLogix.Application.DTOs.CageFusion;
using MLogix.Application.Features.Commands.PredictNuisance;
using MLogix.Application.BackgroundServices;
using MLogix.Application.Features.Commands.DiscloseMolecule;
using MLogix.Application.Features.Queries.GetMolecules.ByRegistrationIDs;
using MLogix.Application.Utils;


namespace MLogix.Application.Features.Commands.RegisterMoleculeBatch
{

    public class RegisterMoleculeBatchHandler : IRequestHandler<RegisterMoleculeBatchCommand, List<RegisterMoleculeResponseDTO>>
    {
        private const int BatchSize = 500;

        private readonly IMapper _mapper;
        private readonly ILogger<RegisterMoleculeBatchHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IEventSourcingHandler<MoleculeAggregate> _eventSourcingHandler;
        private readonly IMoleculeAPI _moleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;
        private readonly INuisanceJobQueue _nuisanceQueue;

        public RegisterMoleculeBatchHandler(
            IMapper mapper,
            ILogger<RegisterMoleculeBatchHandler> logger,
            IMoleculeRepository moleculeRepository,
            IEventSourcingHandler<MoleculeAggregate> eventSourcingHandler,
            IMoleculeAPI moleculeAPI,
            IHttpContextAccessor httpContextAccessor,
            INuisanceJobQueue nuisanceQueue,
            IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _moleculeAPI = moleculeAPI ?? throw new ArgumentNullException(nameof(moleculeAPI));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _nuisanceQueue = nuisanceQueue ?? throw new ArgumentNullException(nameof(nuisanceQueue));
        }

        /*
         * Extracts headers from HTTP context for downstream API calls and tracking.
         */
        private Dictionary<string, string> ExtractRequestHeaders()
        {
            return _httpContextAccessor.HttpContext?.Request?.Headers?
                .ToDictionary(h => h.Key, h => h.Value.ToString()) ?? new Dictionary<string, string>();
        }

        /*
         * Ensures that each command has valid IDs and sets creation metadata.
         */
        private void NormalizeAndAssignCommandIds(IEnumerable<RegisterMoleculeCommandWithRegId> commands, Guid requestorId)
        {
            foreach (var cmd in commands)
            {
                cmd.RegistrationId = cmd.RegistrationId == Guid.Empty ? Guid.NewGuid() : cmd.RegistrationId;
                cmd.Id = cmd.Id == Guid.Empty ? Guid.NewGuid() : cmd.Id;
                cmd.SetCreateProperties(requestorId);
            }
        }

        public async Task<List<RegisterMoleculeResponseDTO>> Handle(RegisterMoleculeBatchCommand request, CancellationToken cancellationToken)
        {

            if (request?.Commands == null || request.Commands.Count == 0)
                throw new ArgumentException("Molecule batch request must contain at least one command.");

            if (request.PreviewMode)
            {
                _logger.LogInformation("Preview mode enabled: No molecules will be registered.");
            }

            var responses = new List<RegisterMoleculeResponseDTO>();
            var headers = ExtractRequestHeaders();

            NormalizeAndAssignCommandIds(commands: request.Commands, requestorId: request.RequestorUserId);

            foreach (var batch in request.Commands.Batch(BatchSize))
            {
                try
                {
                    await ProcessBatchAsync(batch: batch, request: request, headers: headers, responses: responses, isPreviewMode: request.PreviewMode, cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Batch processing failed. Error: {Message}", ex.Message);
                    throw new InvalidOperationException("An error occurred while processing molecule batch.", ex);
                }
            }

            return responses;
        }

        /*
         * Processes a single batch of molecule commands.
         * Delegates logic based on whether SMILES are present or not.
         */
        private async Task ProcessBatchAsync(
            IEnumerable<RegisterMoleculeCommandWithRegId> batch,
            RegisterMoleculeBatchCommand request,
            Dictionary<string, string> headers,
            List<RegisterMoleculeResponseDTO> responses,
            bool isPreviewMode,
            CancellationToken cancellationToken)
        {
            /* Ensure audit metadata is always correct */
            foreach (var cmd in batch)
            {
                cmd.SetCreateProperties(request.RequestorUserId);
            }
            /* Remove if Name is null/empty or white spaces */
            batch = [.. batch.Where(c => !string.IsNullOrWhiteSpace(c.Name))];

            /* Split commands based on SMILES presence */
            var withoutSmiles = batch
                .Where(c => string.IsNullOrWhiteSpace(c.SMILES) && !string.IsNullOrWhiteSpace(c.Name))
                .ToList();

            var withSmiles = batch
                .Where(c => !string.IsNullOrWhiteSpace(c.SMILES))
                .ToList();

            /* Preserve original commands for ID reconciliation */
            var commandSnapshot = new List<RegisterMoleculeCommandWithRegId>(batch);

            if (withoutSmiles.Count > 0)
            {
                await HandleCompoundsWithoutSmilesAsync(
                    compounds: withoutSmiles,
                    headers: headers,
                    responses: responses,
                    isPreviewMode: isPreviewMode,
                    cancellationToken: cancellationToken);
            }

            if (withSmiles.Count > 0)
            {
                await HandleCompoundsWithSmilesAsync(
                    compounds: withSmiles,
                    commandSnapshot: commandSnapshot,
                    request: request,
                    headers: headers,
                    responses: responses,
                    isPreviewMode: isPreviewMode,
                    cancellationToken: cancellationToken);
            }

        }






        /*
         * Handles compounds that do NOT contain SMILES.
         * Performs:
         * 1. ChemVault name/synonym check
         * 2. Local DB lookup
         * 3. Undisclosed registration for true new entries
         */
        private async Task HandleCompoundsWithoutSmilesAsync(
            List<RegisterMoleculeCommandWithRegId> compounds,
            Dictionary<string, string> headers,
            List<RegisterMoleculeResponseDTO> responses,
            bool isPreviewMode,
            CancellationToken cancellationToken)
        {

            /* Deduplicate names for lookup efficiency */
            var namesToCheck = compounds
                .Select(c => c.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Cast<string>()
                .ToList();

            /* ---- ChemVault Check ---- (ChemVault looks for both names and synonyms internally)   */

            /* 1. Remove exact matches from ChemVault */
            var chemVaultMatches = await _moleculeAPI
                .FindByNamesOrSynonymsExact(namesToCheck, headers);

            var chemVaultNames = new HashSet<string>(
                chemVaultMatches.Select(m => m.Name)
                .Concat(chemVaultMatches.SelectMany(m => StringUtilities.ExtractSynonyms(m.Synonyms))),
                StringComparer.OrdinalIgnoreCase);

            compounds.RemoveAll(c => chemVaultNames.Contains(c.Name));

            /* Rehydrate Full molecule details to return in response. get by registrationid*/
            var query = new GetMoleculeByRegistrationIDsQuery
            {
                RegistrationIDs = chemVaultMatches.Select(m => m.Id).ToList()
            };

            var fullDetails = await _mediator.Send(query, cancellationToken);
            responses.AddRange(
                fullDetails.Select(m =>
                {
                    var dto = _mapper.Map<RegisterMoleculeResponseDTO>(m);
                    dto.WasAlreadyRegistered = true;
                    dto.PreviewMessage = "A disclosed molecule with this name (or synonym) already exists.";
                    dto.PreviewStatus = "DUPLICATE_DISCLOSED";
                    return dto;
                }));

            /* At this point compounds contains only those that do not exist in ChemVault */
            /* ---- Local DB Check ---- */

            var existingLocal = await _moleculeRepository.GetByNames(
                compounds.Where(c => !string.IsNullOrWhiteSpace(c.Name))
                .Select(c => c.Name!)
                .ToList());

            var localNames = new HashSet<string>(
                existingLocal.Select(m => m.Name.Value),
                StringComparer.OrdinalIgnoreCase);

            compounds.RemoveAll(c => localNames.Contains(c.Name));

            responses.AddRange(
                existingLocal.Select(m =>
                {
                    var dto = _mapper.Map<RegisterMoleculeResponseDTO>(m);
                    dto.WasAlreadyRegistered = true;
                    dto.PreviewMessage = "An undisclosed molecule with this name already exists.";
                    dto.PreviewStatus = "DUPLICATE_UNDISCLOSED";
                    return dto;
                }));


            /* ---- Register Truly New as Undisclosed ---- */
            if (compounds.Count == 0)
                return;

            _logger.LogInformation(
                "Registering {Count} new undisclosed compounds.",
                compounds.Count);

            var undisclosedCommands = compounds
                .Select(c => _mapper.Map<RegisterUndisclosedCommand>(c))
                .ToList();

            await ProcessUndisclosedMoleculesAsync(
                undisclosedBatch: undisclosedCommands,
                allResponses: responses,
                isPreviewMode: isPreviewMode,
                cancellationToken: cancellationToken);
        }


        /*
         * Processes undisclosed molecule registrations one-by-one via Mediator.
         * Skips ChemVault checks as already handled upstream.
         */
        private async Task ProcessUndisclosedMoleculesAsync(
            List<RegisterUndisclosedCommand> undisclosedBatch,
            List<RegisterMoleculeResponseDTO> allResponses,
            bool isPreviewMode,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Processing {Count} undisclosed molecules...",
                undisclosedBatch.Count);

            foreach (var cmd in undisclosedBatch)
            {
                cmd.ChemVaultCheck = false;
                cmd.PreviewMode = isPreviewMode;
                var result = await _mediator.Send(cmd, cancellationToken);
                var responseDto = _mapper.Map<RegisterMoleculeResponseDTO>(result);

                allResponses.Add(responseDto);
            }
        }



        /*
         * Handles compounds that contain SMILES.
         *
         * Workflow:
         * 1. Check ChemVault by SMILES
         * 2. Remove already-registered molecules
         * 3. Disclose previously undisclosed molecules
         * 4. Register new molecules in ChemVault
         * 5. Persist events locally
         * 6. Queue nuisance predictions
         */
        private async Task HandleCompoundsWithSmilesAsync(
            List<RegisterMoleculeCommandWithRegId> compounds,
            List<RegisterMoleculeCommandWithRegId> commandSnapshot,
            RegisterMoleculeBatchCommand request,
            Dictionary<string, string> headers,
            List<RegisterMoleculeResponseDTO> responses,
            bool isPreviewMode,
            CancellationToken cancellationToken)
        {
            var nuisanceQueue = new List<NuisanceRequestTuple>();
            var utcNow = DateTime.UtcNow;

            /* -------------------------------------------------
             * STEP 1: ChemVault SMILES lookup
             * ------------------------------------------------- */

            var smilesToCheck = compounds
               .Select(c => c.SMILES)
               .Where(s => !string.IsNullOrWhiteSpace(s))
               .Distinct()
               .Cast<string>()
               .ToList();

            var chemVaultMatches = await _moleculeAPI
                .GetMoleculesBySMILES(smilesToCheck, headers);

            // Now if we find direct name or synonym matches in ChemVault, we will consider them existing and remove them from registration list
            var chemVaultNames = new HashSet<string>(
                chemVaultMatches.Select(m => m.Name)
                    .Concat(chemVaultMatches.SelectMany(m => StringUtilities.ExtractSynonyms(m.Synonyms))),
                StringComparer.OrdinalIgnoreCase);

            if (chemVaultNames.Count > 0)
            {
                // Find compounds that are being removed due to ChemVault match
                var compoundsToRemove = compounds
                    .Where(c => c.Name != null && chemVaultNames.Contains(c.Name))
                    .ToList();

                compounds.RemoveAll(c => c.Name != null && chemVaultNames.Contains(c.Name)); // Remove exact matches

                if (compoundsToRemove.Count > 0)
                {
                    // Rehydrate Full molecule details to return in response.
                    var query = new GetMoleculeByRegistrationIDsQuery
                    {
                        RegistrationIDs = chemVaultMatches.Select(m => m.Id).ToList()
                    };

                    var existingLocal = await _mediator.Send(query, cancellationToken);

                    // Only add responses for compounds that were actually removed
                    foreach (var removed in compoundsToRemove)
                    {
                        var match = existingLocal
                            .FirstOrDefault(m =>
                                string.Equals(m.Name, removed.Name, StringComparison.OrdinalIgnoreCase) ||
                                (m.Synonyms != null && StringUtilities.ExtractSynonyms(m.Synonyms)
                                    .Any(syn => string.Equals(syn, removed.Name, StringComparison.OrdinalIgnoreCase))));
                        if (match != null)
                        {
                            var dto = _mapper.Map<RegisterMoleculeResponseDTO>(match);
                            dto.WasAlreadyRegistered = true;
                            dto.PreviewMessage = "A disclosed molecule with this name (or synonym) already exists.";
                            dto.PreviewStatus = "DUPLICATE_DISCLOSED";
                            responses.Add(dto);
                        }
                    }
                }
            }

            // Quick return if the compounds list is now empty
            if (compounds.Count == 0)
                return;



            /* -------------------------------------------------
             * STEP 2: Disclosure of existing undisclosed molecules
             * ------------------------------------------------- */

            var namesToCheck = compounds
                .Select(c => c.Name)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var existingLocalByName = await _moleculeRepository.GetByNames(namesToCheck);

            var undisclosedNames = existingLocalByName
                .Where(m => string.IsNullOrWhiteSpace(m.RequestedSMILES))
                .Select(m => m.Name.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var toDisclose = compounds
                .Where(c => undisclosedNames.Contains(c.Name))
                .ToList();

            if (toDisclose.Count > 0)
            {
                /* Rehydrate IDs from existing records */
                foreach (var cmd in toDisclose)
                {
                    var existing = existingLocalByName
                        .First(m => string.Equals(m.Name, cmd.Name, StringComparison.OrdinalIgnoreCase));

                    cmd.Id = existing.Id;
                    cmd.RegistrationId = existing.RegistrationId;
                }

                compounds.RemoveAll(c => undisclosedNames.Contains(c.Name));

                var discloseBatch = new DiscloseMoleculeBatchCommand
                {
                    Molecules = toDisclose
                        .Select(c => _mapper.Map<DiscloseMoleculeCommand>(c))
                        .ToList()
                    ,
                    PreviewMode = isPreviewMode
                    ,
                    SkipNuisanceCheck = true
                };

                var disclosed = await _mediator.Send(discloseBatch, cancellationToken);

                responses.AddRange(
                disclosed.Select(m =>
                    {
                        var dto = _mapper.Map<RegisterMoleculeResponseDTO>(m);
                        dto.PreviewMessage = "The molecule will be disclosed.";
                        dto.PreviewStatus = "DISCLOSURE";
                        return dto;
                    }));


                if (!isPreviewMode)
                {
                    nuisanceQueue.AddRange(
                    disclosed.Select(d => new NuisanceRequestTuple
                    {
                        Id = d.Id.ToString(),
                        SMILES = d.SmilesCanonical
                    }));

                }
            }

            // Quick return if now compounds is empty
            if (compounds.Count == 0)
            {
                if (!isPreviewMode)
                    await QueueNuisancePredictionsAsync(
                        nuisanceQueue,
                        request,
                        utcNow,
                        cancellationToken);
                return;
            }

            /* -------------------------------------------------
             * STEP 3: Register new molecules in ChemVault
             * ------------------------------------------------- */

            var chemVaultRequests = compounds.Select(cmd =>
            {
                var mapped = _mapper.Map<RegisterMoleculeCommandWithRegId>(cmd);
                mapped.Id = cmd.RegistrationId; // ChemVault expects registration ID
                return mapped;
            }).ToList();

            var chemVaultResponses = await _moleculeAPI
                .RegisterBatch(registerMoleculeCommands: chemVaultRequests, previewMode: isPreviewMode, headers: headers);


            foreach (var apiResponse in chemVaultResponses)
            {
                var existingLocal = await _moleculeRepository.GetMoleculeByRegistrationId(apiResponse.Id);
                if (existingLocal != null)
                {
                    var dto = _mapper.Map<RegisterMoleculeResponseDTO>(apiResponse);
                    dto.WasAlreadyRegistered = true;
                    dto.Id = existingLocal.Id;
                    dto.RegistrationId = apiResponse.Id;
                    dto.PreviewMessage = "This molecule is already disclosed and registered in ChemVault. Any differing submitted name will be added as a synonym.";
                    dto.PreviewStatus = "DUPLICATE_DISCLOSED";
                    responses.Add(dto);
                    continue;
                }


                /* -------------------------------------------------
                 * STEP 4: Event sourcing (Created + Disclosed)
                 * ------------------------------------------------- */
                
                var originalCmd = commandSnapshot
                  .FirstOrDefault(c => c.RegistrationId == apiResponse.Id);
                if (originalCmd == null)
                {
                    // This is a failsafe case and should not happen in normal scenarios unless ChemVault has drifted away from MLogix
                    _logger.LogWarning(
                        "ChemVault Drift Detected: Molecule in ChemVault With Registration ID: {RegId}, Name {name}, but NOT found in MLogix",
                        apiResponse.Id, apiResponse.Name);
                    continue;
                }

                var moleculeCreatedEvent = _mapper.Map<MoleculeCreatedEvent>(apiResponse);
                moleculeCreatedEvent.RequestorUserId = request.RequestorUserId;
                moleculeCreatedEvent.CreatedById = request.RequestorUserId;
                moleculeCreatedEvent.DateCreated = utcNow;
                moleculeCreatedEvent.IsModified = false;
                moleculeCreatedEvent.LastModifiedById = request.RequestorUserId;
                moleculeCreatedEvent.DateModified = utcNow;
                moleculeCreatedEvent.Id = originalCmd.Id;
                moleculeCreatedEvent.RequestedSMILES = originalCmd.SMILES;
                moleculeCreatedEvent.RegistrationId = apiResponse.Id;
                moleculeCreatedEvent.SmilesCanonical = apiResponse.SmilesCanonical;


                var response = _mapper.Map<RegisterMoleculeResponseDTO>(apiResponse);
                response.Id = moleculeCreatedEvent.Id;
                response.WasAlreadyRegistered = false;
                response.PreviewMessage = "This molecule would be registered and disclosed.";
                response.PreviewStatus = "REGISTRATION";

                responses.Add(response);

                if (!isPreviewMode)
                {
                    var aggregate = new MoleculeAggregate(moleculeCreatedEvent);

                    // Now lets create Disclosure Event as these all have SMILES
                    var disclosedEvent = BuildMoleculeDisclosedEvent(
                        moleculeCreatedEvent,
                        originalCmd,
                        headers,
                        request.RequestorUserId,
                        utcNow);

                    aggregate.DiscloseMolecule(disclosedEvent);
                    await _eventSourcingHandler.SaveAsync(aggregate);

                    nuisanceQueue.Add(new NuisanceRequestTuple
                    {
                        Id = moleculeCreatedEvent.Id.ToString(),
                        SMILES = response.SmilesCanonical
                    });


                    await QueueNuisancePredictionsAsync(
                    nuisanceQueue,
                    request,
                    utcNow,
                    cancellationToken);
                }

            }


        }

        /*
         * Builds a MoleculeDisclosedEvent for a new molecule.
         * Pulls disclosure metadata from headers or command.
         */
        private MoleculeDisclosedEvent BuildMoleculeDisclosedEvent(
            MoleculeCreatedEvent createdEvent,
            RegisterMoleculeCommandWithRegId originalCmd,
            Dictionary<string, string> headers,
            Guid requestorUserId,
            DateTime utcNow)
        {
            var disclosureScientist = originalCmd.DisclosureScientist;
            if (string.IsNullOrWhiteSpace(disclosureScientist) &&
                headers.TryGetValue("AppUser-FullName", out var nameFromHeader))
            {
                disclosureScientist = nameFromHeader;
            }

            var disclosureOrgId = originalCmd.DisclosureOrgId;
            if (disclosureOrgId == Guid.Empty &&
                headers.TryGetValue("AppOrg-Id", out var orgIdFromHeader) &&
                Guid.TryParse(orgIdFromHeader, out var parsedOrgId))
            {
                disclosureOrgId = parsedOrgId;
            }

            return new MoleculeDisclosedEvent
            {
                RequestorUserId = requestorUserId,
                CreatedById = requestorUserId,
                LastModifiedById = requestorUserId,
                DateCreated = utcNow,
                DateModified = utcNow,
                IsModified = false,

                RegistrationId = createdEvent.RegistrationId,
                Name = createdEvent.Name,
                RequestedSMILES = createdEvent.RequestedSMILES,
                SmilesCanonical = createdEvent.SmilesCanonical,
                IsStructureDisclosed = true,
                StructureDisclosedDate = originalCmd.StructureDisclosedDate != default
                    ? originalCmd.StructureDisclosedDate
                    : utcNow,
                StructureDisclosedByUserId = requestorUserId,

                DisclosureScientist = disclosureScientist,
                DisclosureOrgId = disclosureOrgId,
                DisclosureStage = originalCmd.DisclosureStage,
                DisclosureReason = string.IsNullOrWhiteSpace(originalCmd.DisclosureReason)
                    ? "Automatic registration"
                    : originalCmd.DisclosureReason,
                DisclosureNotes = originalCmd.DisclosureNotes,
                LiteratureReferences = originalCmd.LiteratureReferences
            };
        }


        /*
         * Queues nuisance predictions via INuisanceJobQueue.
         * Catches/logs any failure gracefully without breaking the main flow.
         */
        private async Task QueueNuisancePredictionsAsync(
            List<NuisanceRequestTuple> nuisanceList,
            RegisterMoleculeBatchCommand request,
            DateTime utcNow,
            CancellationToken cancellationToken)
        {
            if (nuisanceList.Count == 0)
                return;

            var nuisanceCommand = new PredictNuisanceCommand
            {
                NuisanceRequestTuple = nuisanceList,
                PlotAllAttention = false,
                RequestorUserId = request.RequestorUserId,
                CreatedById = request.RequestorUserId,
                LastModifiedById = request.RequestorUserId,
                DateCreated = utcNow,
                DateModified = utcNow,
                IsModified = false,
            };

            try
            {
                var correlationId = Guid.NewGuid().ToString("N");
                using (_logger.BeginScope(new Dictionary<string, object> { ["corrId"] = correlationId }))
                {
                    var job = new NuisanceJob(nuisanceCommand, correlationId);
                    await _nuisanceQueue.EnqueueAsync(job, cancellationToken);

                    _logger.LogInformation(
                        "Queued nuisance prediction for {Count} molecules.",
                        nuisanceList.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to queue nuisance prediction job.");
            }
        }

    }
}